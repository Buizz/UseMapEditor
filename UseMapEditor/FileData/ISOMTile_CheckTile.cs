using Data.Map;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows.Forms.VisualStyles;
using UseMapEditor.Control;
using static UseMapEditor.FileData.TileSet;

namespace UseMapEditor.FileData
{
    public partial class ISOMTile
    {
        public TileBorder CheckTile(ushort LT, ushort RT, ushort LB, ushort RB, int tx, int ty, int mapheight)
        {
            bool isFlatLT = flat_mtxmlist.ContainsKey(LT);
            bool isFlatRT = flat_mtxmlist.ContainsKey(RT);
            bool isFlatLB = flat_mtxmlist.ContainsKey(LB);
            bool isFlatRB = flat_mtxmlist.ContainsKey(RB);

            bool isEdgeLT = edge_mtxmlist.ContainsKey(LT);
            bool isEdgeRT = edge_mtxmlist.ContainsKey(RT);
            bool isEdgeLB = edge_mtxmlist.ContainsKey(LB);
            bool isEdgeRB = edge_mtxmlist.ContainsKey(RB);

            
            if (ty < -1)
            {
                if (ConnectHighTile != null && !ConnectHighTile.IsMiniISOM)
                {
                    bool isuEdgeLT = ConnectHighTile.edge_top_connect_mtxmlist.ContainsKey(LT);
                    bool isuEdgeRT = ConnectHighTile.edge_top_connect_mtxmlist.ContainsKey(RT);
                    bool isuEdgeLB = ConnectHighTile.edge_top_connect_mtxmlist.ContainsKey(LB);
                    bool isuEdgeRB = ConnectHighTile.edge_top_connect_mtxmlist.ContainsKey(RB);


                    bool _isCliffLT = ConnectHighTile.cliff_donwmtxmlist.ContainsKey(LT);
                    bool _isCliffRT = ConnectHighTile.cliff_donwmtxmlist.ContainsKey(RT);
                    bool _isCliffLB = ConnectHighTile.cliff_donwmtxmlist.ContainsKey(LB);
                    bool _isCliffRB = ConnectHighTile.cliff_donwmtxmlist.ContainsKey(RB);


                    bool _isCliffDownLT = ConnectHighTile.cliff_down_mtxmlist.ContainsKey(LT);
                    bool _isCliffDownRT = ConnectHighTile.cliff_down_mtxmlist.ContainsKey(RT);
                    bool _isCliffDownLB = ConnectHighTile.cliff_down_mtxmlist.ContainsKey(LB);
                    bool _isCliffDownRB = ConnectHighTile.cliff_down_mtxmlist.ContainsKey(RB);


                    if (isuEdgeLT || isuEdgeRT || isuEdgeLB || isuEdgeRB
                        || _isCliffLT || _isCliffRT || _isCliffLB || _isCliffRB
                         || _isCliffDownLT || _isCliffDownRT || _isCliffDownLB || _isCliffDownRB)
                    {
                        return TileBorder.None;
                    }
                }

                if (ConnectLowTile != null)
                {
                    bool _isCliffLT = cliff_donwmtxmlist.ContainsKey(LT);
                    bool _isCliffRT = cliff_donwmtxmlist.ContainsKey(RT);
                    bool _isCliffLB = cliff_donwmtxmlist.ContainsKey(LB);
                    bool _isCliffRB = cliff_donwmtxmlist.ContainsKey(RB);

                    if (_isCliffLT || _isCliffRT || _isCliffLB || _isCliffRB)
                    {
                        return TileBorder.DownBorder;
                    }
                }


                if (!IsMiniISOM)
                {
                    if (isEdgeLT || isEdgeRT || isEdgeLB || isEdgeRB)
                    {
                        return TileBorder.DownBorder;
                    }
                }


                //if(isFlatLT || isFlatRT || isFlatLB || isFlatRB)
                //{
                //    return TileBorder.Flat;
                //}
            }
            else if (ty == -1)
            {
                if (ConnectHighTile != null && !ConnectHighTile.IsMiniISOM)
                {
                    bool _isCliffLT = ConnectHighTile.cliff_middlebottom_mtxmlist.ContainsKey(LT);
                    bool _isCliffRT = ConnectHighTile.cliff_middlebottom_mtxmlist.ContainsKey(RT);
                    bool _isCliffLB = ConnectHighTile.cliff_middlebottom_mtxmlist.ContainsKey(LB);
                    bool _isCliffRB = ConnectHighTile.cliff_middlebottom_mtxmlist.ContainsKey(RB);

                    bool _isEdgeLT = ConnectHighTile.edge_nocorner_mtxmlist.ContainsKey(LT);
                    bool _isEdgeRT = ConnectHighTile.edge_nocorner_mtxmlist.ContainsKey(RT);
                    bool _isEdgeLB = ConnectHighTile.edge_nocorner_mtxmlist.ContainsKey(LB);
                    bool _isEdgeRB = ConnectHighTile.edge_nocorner_mtxmlist.ContainsKey(RB);

                    bool _isCliffDownLT = ConnectHighTile.cliff_donwmtxmlist.ContainsKey(LT);
                    bool _isCliffDownRT = ConnectHighTile.cliff_donwmtxmlist.ContainsKey(RT);
                    bool _isCliffDownLB = ConnectHighTile.cliff_donwmtxmlist.ContainsKey(LB);
                    bool _isCliffDownRB = ConnectHighTile.cliff_donwmtxmlist.ContainsKey(RB);

                    if (_isCliffDownLT || _isCliffDownRT || _isCliffDownLB || _isCliffDownRB)
                    {
                        return TileBorder.None;
                    }

                    if (_isCliffLT || _isCliffRT || _isCliffLB || _isCliffRB)
                    {
                        return TileBorder.None;
                    }

                    if (_isEdgeLT && _isEdgeRT && _isEdgeLB && _isEdgeRB)
                    {
                        return TileBorder.Flat;
                    }
                    if (_isEdgeLT && !_isEdgeRT && _isEdgeLB && !_isEdgeRB)
                    {
                        return TileBorder.Flat;
                    }
                    if (!_isEdgeLT && _isEdgeRT && !_isEdgeLB && _isEdgeRB)
                    {
                        return TileBorder.Flat;
                    }
                }


                if (ConnectLowTile != null)
                {
                    bool _isCliffLT = cliff_donwmtxmlist.ContainsKey(LT);
                    bool _isCliffRT = cliff_donwmtxmlist.ContainsKey(RT);
                    bool _isCliffLB = cliff_donwmtxmlist.ContainsKey(LB);
                    bool _isCliffRB = cliff_donwmtxmlist.ContainsKey(RB);

                    if (_isCliffLT || _isCliffRT || _isCliffLB || _isCliffRB)
                    {
                        return TileBorder.DownBorder;
                    }
                }
            }
            
            if(ty == mapheight)
            {
                if (ConnectHighTile != null && !ConnectHighTile.IsMiniISOM)
                {
                    if (ConnectHighTile.cliff_middlebottom_mtxmlist.ContainsKey(LT)
                        || ConnectHighTile.cliff_middlebottom_mtxmlist.ContainsKey(RT)
                        || ConnectHighTile.cliff_middlebottom_mtxmlist.ContainsKey(LB)
                        || ConnectHighTile.cliff_middlebottom_mtxmlist.ContainsKey(RB))
                    {
                        return TileBorder.Flat;
                    }
                }
            }



            if (topdonwlist.ContainsKey(LT)) return TileBorder.DownBorder;
            else if (topdonwlist.ContainsKey(RT)) return TileBorder.DownBorder;
            else if (topdonwlist.ContainsKey(LB)) return TileBorder.DownBorder;
            else if (topdonwlist.ContainsKey(RB)) return TileBorder.DownBorder;

            if (ConnectHighTile != null && !ConnectHighTile.IsMiniISOM)
            {
                if (isEdgeLB && isEdgeRB
                    && (ConnectHighTile.tip_right_mtxmlist.ContainsKey(RT) || ConnectHighTile.cliff_flat_right_mtxmlist.ContainsKey(RT))
                    && (ConnectHighTile.tip_left_mtxmlist.ContainsKey(LT) || ConnectHighTile.cliff_flat_left_mtxmlist.ContainsKey(LT)))
                {
                    return TileBorder.DownBorder;
                }
            }



            //4개가 전부 플랫 타일일 경우 (무조건 플랫타일)
            if (isFlatLT && isFlatRT && isFlatLB && isFlatRB) return TileBorder.Flat;


            //하나라도 플랫 타일이 있을 경우
            if (isFlatLT || isFlatRT || isFlatLB || isFlatRB)
            {
                bool alledge = true;
                if (!isFlatLT) alledge &= isEdgeLT;
                if (!isFlatRT) alledge &= isEdgeRT;
                if (!isFlatLB) alledge &= isEdgeLB;
                if (!isFlatRB) alledge &= isEdgeRB;

                //1. 나머지 타일이 전부 해당 타일의 Edge일 경우 -> 경계타일
                if (alledge)
                {
                    return TileBorder.DownBorder;
                }

                //2. 나머지 타일이 전부 HighTile의 Edge일 경우 -> 플랫타일
                if (ConnectHighTile != null)
                {
                    alledge = true;
                    if (!isFlatLT) alledge &= ConnectHighTile.edge_mtxmlist.ContainsKey(LT);
                    if (!isFlatRT) alledge &= ConnectHighTile.edge_mtxmlist.ContainsKey(RT);
                    if (!isFlatLB) alledge &= ConnectHighTile.edge_mtxmlist.ContainsKey(LB);
                    if (!isFlatRB) alledge &= ConnectHighTile.edge_mtxmlist.ContainsKey(RB);

                    if (alledge)
                    {
                        return TileBorder.Flat;
                    }

                    if (ConnectHighTile.edge_mtxmlist.ContainsKey(LT) && ConnectHighTile.edge_mtxmlist.ContainsKey(RT))
                    {
                        if (isFlatLB && isEdgeRB)
                        {
                            return TileBorder.DownBorder;
                        }
                        else if (isFlatRB && isEdgeLB)
                        {
                            return TileBorder.DownBorder;
                        }
                    }

                }
            }



            //하나라도 플랫 타일이 없을 경우
            //1. 4개가 LT,LB는 HighTile의 Cliff, RT,RB가 HighTile의 Cliff일 경우 플랫타일
            if (ConnectHighTile != null)
            {
                if (ConnectHighTile.cliff_flat_left_mtxmlist.ContainsKey(RT)
                    && ConnectHighTile.cliff_flat_right_mtxmlist.ContainsKey(LT)
                    && ConnectHighTile.edge_mtxmlist.ContainsKey(RB)
                    && ConnectHighTile.edge_mtxmlist.ContainsKey(LB)
                    && ConnectHighTile.cliff_flat_left_mtxmlist.ContainsKey(RB)
                    && ConnectHighTile.cliff_flat_right_mtxmlist.ContainsKey(LB)
                    && ty != 0
                    )
                {
                    return TileBorder.Flat;
                }


                //if (connecthightile.tip_left_mtxmlist.ContainsKey(RT)
                //    && connecthightile.tip_left_mtxmlist.ContainsKey(RB)
                //    && connecthightile.tip_right_mtxmlist.ContainsKey(LT)
                //    && connecthightile.tip_right_mtxmlist.ContainsKey(LB))
                //{
                //    return TileBorder.Flat;
                //}
            }


            //2. 모두 Edge일 경우 -> 경계타일
            if (isEdgeLT && isEdgeRT && isEdgeLB && isEdgeRB)
            {
                //모두 Tip일 경우 아무것도 아님.
                if (!tip_left_mtxmlist.ContainsKey(RT) && !tip_left_mtxmlist.ContainsKey(RB) && !tip_right_mtxmlist.ContainsKey(LT) && !tip_right_mtxmlist.ContainsKey(LB))
                {
                    return TileBorder.DownBorder;
                }
            }




            //그 외는 None처리
            return TileBorder.None;
        }
    }
}
