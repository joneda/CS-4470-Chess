using System;
using System.Collections.Generic;
using System.Text;

namespace UvsChess.Framework
{
    internal enum ProfilerMethodKey:int
    {
        ChessBoard_ctor,
        ChessBoard_ctor_string,
        ChessBoard_ctor_ChessPieceArray,
        ChessBoard_get_ChessLocationIndexer,
        ChessBoard_set_ChessLocationIndexer,
        ChessBoard_get_Indexer_int_int,
        ChessBoard_set_Indexer_int_int,
        ChessBoard_get_RawBoard,
        ChessBoard_CloneBoard_ChessPieceArray,
        ChessBoard_Clone,
        ChessBoard_MakeMove_ChessMove,
        ChessBoard_FromFenBoard_string,
        ChessBoard_ToPartialFenBoard,
        ChessBoard_GetHashCode,

        ChessLocation_ctor_int_int,
        ChessLocation_get_IsValid,
        ChessLocation_Clone,
        ChessLocation_Equals,
        ChessLocation_NE,
        ChessLocation_EQ,
        ChessLocation_GetHashCode,

        ChessMove_ctor_ChessLocation_ChessLocation,
        ChessMove_ctor_ChessLocation_ChessLocation_ChessFlag,
        ChessMove_get_IsBasicallyValid,
        ChessMove_Clone,
        ChessMove_ToString,
        ChessMove_Equals,
        ChessMove_EQ,
        ChessMove_NE,
        ChessMove_CompareTo_ChessMove,
        ChessMove_GetHashCode,

        DecisionTree_ctor_ChessBoard,
        DecisionTree_ctor_DecisionTree_ChessBoard_ChessMove,
        DecisionTree_get_LastChild,
        DecisionTree_get_ActualMoveValue,
        DecisionTree_get_EventualMoveValue,
        DecisionTree_get_IsRootNode,
        DecisionTree_get_Board,
        DecisionTree_set_Board,
        DecisionTree_get_Move,
        DecisionTree_set_Move,
        DecisionTree_Clone,
        DecisionTree_Clone_DecisionTree,
        DecisionTree_AddChild_ChessBoard_ChessMove,
        DecisionTree_ToString,
    }
}

