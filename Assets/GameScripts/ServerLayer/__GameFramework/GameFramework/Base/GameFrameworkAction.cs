﻿//------------------------------------------------------------
// Game Framework
// Copyright © 2013-2021 Jiang Yin. All rights reserved.
// Homepage: https://gameframework.cn/
// Feedback: mailto:ellan@gameframework.cn
//------------------------------------------------------------

namespace GameFramework
{
    /// <summary>
    /// 封装一个方法，该方法不具有参数并且不返回值。
    /// </summary>
    public delegate void GameFrameworkAction();

    /// <summary>
    /// 封装一个方法，该方法只有一个参数并且不返回值。
    /// </summary>
    /// <typeparam name="T">此委托封装的方法的参数类型。</typeparam>
    /// <param name="obj">此委托封装的方法的参数。</param>
    public delegate void GameFrameworkAction<in T>(T obj);

    /// <summary>
    /// 封装一个方法，该方法具有两个参数并且不返回值。
    /// </summary>
    /// <typeparam name="T1">此委托封装的方法的第一个参数的类型。</typeparam>
    /// <typeparam name="T2">此委托封装的方法的第二个参数的类型。</typeparam>
    /// <param name="arg1">此委托封装的方法的第一个参数。</param>
    /// <param name="arg2">此委托封装的方法的第二个参数。</param>
    public delegate void GameFrameworkAction<in T1, in T2>(T1 arg1, T2 arg2);

    /// <summary>
    /// 封装一个方法，该方法具有三个参数并且不返回值。
    /// </summary>
    /// <typeparam name="T1">此委托封装的方法的第一个参数的类型。</typeparam>
    /// <typeparam name="T2">此委托封装的方法的第二个参数的类型。</typeparam>
    /// <typeparam name="T3">此委托封装的方法的第三个参数的类型。</typeparam>
    /// <param name="arg1">此委托封装的方法的第一个参数。</param>
    /// <param name="arg2">此委托封装的方法的第二个参数。</param>
    /// <param name="arg3">此委托封装的方法的第三个参数。</param>
    public delegate void GameFrameworkAction<in T1, in T2, in T3>(T1 arg1, T2 arg2, T3 arg3);

    /// <summary>
    /// 封装一个方法，该方法具有四个参数并且不返回值。
    /// </summary>
    /// <typeparam name="T1">此委托封装的方法的第一个参数的类型。</typeparam>
    /// <typeparam name="T2">此委托封装的方法的第二个参数的类型。</typeparam>
    /// <typeparam name="T3">此委托封装的方法的第三个参数的类型。</typeparam>
    /// <typeparam name="T4">此委托封装的方法的第四个参数的类型。</typeparam>
    /// <param name="arg1">此委托封装的方法的第一个参数。</param>
    /// <param name="arg2">此委托封装的方法的第二个参数。</param>
    /// <param name="arg3">此委托封装的方法的第三个参数。</param>
    /// <param name="arg4">此委托封装的方法的第四个参数。</param>
    public delegate void GameFrameworkAction<in T1, in T2, in T3, in T4>(T1 arg1, T2 arg2, T3 arg3, T4 arg4);

    /// <summary>
    /// 封装一个方法，该方法具有五个参数并且不返回值。
    /// </summary>
    /// <typeparam name="T1">此委托封装的方法的第一个参数的类型。</typeparam>
    /// <typeparam name="T2">此委托封装的方法的第二个参数的类型。</typeparam>
    /// <typeparam name="T3">此委托封装的方法的第三个参数的类型。</typeparam>
    /// <typeparam name="T4">此委托封装的方法的第四个参数的类型。</typeparam>
    /// <typeparam name="T5">此委托封装的方法的第五个参数的类型。</typeparam>
    /// <param name="arg1">此委托封装的方法的第一个参数。</param>
    /// <param name="arg2">此委托封装的方法的第二个参数。</param>
    /// <param name="arg3">此委托封装的方法的第三个参数。</param>
    /// <param name="arg4">此委托封装的方法的第四个参数。</param>
    /// <param name="arg5">此委托封装的方法的第五个参数。</param>
    public delegate void GameFrameworkAction<in T1, in T2, in T3, in T4, in T5>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5);

    /// <summary>
    /// 封装一个方法，该方法具有六个参数并且不返回值。
    /// </summary>
    /// <typeparam name="T1">此委托封装的方法的第一个参数的类型。</typeparam>
    /// <typeparam name="T2">此委托封装的方法的第二个参数的类型。</typeparam>
    /// <typeparam name="T3">此委托封装的方法的第三个参数的类型。</typeparam>
    /// <typeparam name="T4">此委托封装的方法的第四个参数的类型。</typeparam>
    /// <typeparam name="T5">此委托封装的方法的第五个参数的类型。</typeparam>
    /// <typeparam name="T6">此委托封装的方法的第六个参数的类型。</typeparam>
    /// <param name="arg1">此委托封装的方法的第一个参数。</param>
    /// <param name="arg2">此委托封装的方法的第二个参数。</param>
    /// <param name="arg3">此委托封装的方法的第三个参数。</param>
    /// <param name="arg4">此委托封装的方法的第四个参数。</param>
    /// <param name="arg5">此委托封装的方法的第五个参数。</param>
    /// <param name="arg6">此委托封装的方法的第六个参数。</param>
    public delegate void GameFrameworkAction<in T1, in T2, in T3, in T4, in T5, in T6>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6);

    /// <summary>
    /// 封装一个方法，该方法具有七个参数并且不返回值。
    /// </summary>
    /// <typeparam name="T1">此委托封装的方法的第一个参数的类型。</typeparam>
    /// <typeparam name="T2">此委托封装的方法的第二个参数的类型。</typeparam>
    /// <typeparam name="T3">此委托封装的方法的第三个参数的类型。</typeparam>
    /// <typeparam name="T4">此委托封装的方法的第四个参数的类型。</typeparam>
    /// <typeparam name="T5">此委托封装的方法的第五个参数的类型。</typeparam>
    /// <typeparam name="T6">此委托封装的方法的第六个参数的类型。</typeparam>
    /// <typeparam name="T7">此委托封装的方法的第七个参数的类型。</typeparam>
    /// <param name="arg1">此委托封装的方法的第一个参数。</param>
    /// <param name="arg2">此委托封装的方法的第二个参数。</param>
    /// <param name="arg3">此委托封装的方法的第三个参数。</param>
    /// <param name="arg4">此委托封装的方法的第四个参数。</param>
    /// <param name="arg5">此委托封装的方法的第五个参数。</param>
    /// <param name="arg6">此委托封装的方法的第六个参数。</param>
    /// <param name="arg7">此委托封装的方法的第七个参数。</param>
    public delegate void GameFrameworkAction<in T1, in T2, in T3, in T4, in T5, in T6, in T7>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7);

    /// <summary>
    /// 封装一个方法，该方法具有八个参数并且不返回值。
    /// </summary>
    /// <typeparam name="T1">此委托封装的方法的第一个参数的类型。</typeparam>
    /// <typeparam name="T2">此委托封装的方法的第二个参数的类型。</typeparam>
    /// <typeparam name="T3">此委托封装的方法的第三个参数的类型。</typeparam>
    /// <typeparam name="T4">此委托封装的方法的第四个参数的类型。</typeparam>
    /// <typeparam name="T5">此委托封装的方法的第五个参数的类型。</typeparam>
    /// <typeparam name="T6">此委托封装的方法的第六个参数的类型。</typeparam>
    /// <typeparam name="T7">此委托封装的方法的第七个参数的类型。</typeparam>
    /// <typeparam name="T8">此委托封装的方法的第八个参数的类型。</typeparam>
    /// <param name="arg1">此委托封装的方法的第一个参数。</param>
    /// <param name="arg2">此委托封装的方法的第二个参数。</param>
    /// <param name="arg3">此委托封装的方法的第三个参数。</param>
    /// <param name="arg4">此委托封装的方法的第四个参数。</param>
    /// <param name="arg5">此委托封装的方法的第五个参数。</param>
    /// <param name="arg6">此委托封装的方法的第六个参数。</param>
    /// <param name="arg7">此委托封装的方法的第七个参数。</param>
    /// <param name="arg8">此委托封装的方法的第八个参数。</param>
    public delegate void GameFrameworkAction<in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8);

    /// <summary>
    /// 封装一个方法，该方法具有九个参数并且不返回值。
    /// </summary>
    /// <typeparam name="T1">此委托封装的方法的第一个参数的类型。</typeparam>
    /// <typeparam name="T2">此委托封装的方法的第二个参数的类型。</typeparam>
    /// <typeparam name="T3">此委托封装的方法的第三个参数的类型。</typeparam>
    /// <typeparam name="T4">此委托封装的方法的第四个参数的类型。</typeparam>
    /// <typeparam name="T5">此委托封装的方法的第五个参数的类型。</typeparam>
    /// <typeparam name="T6">此委托封装的方法的第六个参数的类型。</typeparam>
    /// <typeparam name="T7">此委托封装的方法的第七个参数的类型。</typeparam>
    /// <typeparam name="T8">此委托封装的方法的第八个参数的类型。</typeparam>
    /// <typeparam name="T9">此委托封装的方法的第九个参数的类型。</typeparam>
    /// <param name="arg1">此委托封装的方法的第一个参数。</param>
    /// <param name="arg2">此委托封装的方法的第二个参数。</param>
    /// <param name="arg3">此委托封装的方法的第三个参数。</param>
    /// <param name="arg4">此委托封装的方法的第四个参数。</param>
    /// <param name="arg5">此委托封装的方法的第五个参数。</param>
    /// <param name="arg6">此委托封装的方法的第六个参数。</param>
    /// <param name="arg7">此委托封装的方法的第七个参数。</param>
    /// <param name="arg8">此委托封装的方法的第八个参数。</param>
    /// <param name="arg9">此委托封装的方法的第九个参数。</param>
    public delegate void GameFrameworkAction<in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8, in T9>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9);

    /// <summary>
    /// 封装一个方法，该方法具有十个参数并且不返回值。
    /// </summary>
    /// <typeparam name="T1">此委托封装的方法的第一个参数的类型。</typeparam>
    /// <typeparam name="T2">此委托封装的方法的第二个参数的类型。</typeparam>
    /// <typeparam name="T3">此委托封装的方法的第三个参数的类型。</typeparam>
    /// <typeparam name="T4">此委托封装的方法的第四个参数的类型。</typeparam>
    /// <typeparam name="T5">此委托封装的方法的第五个参数的类型。</typeparam>
    /// <typeparam name="T6">此委托封装的方法的第六个参数的类型。</typeparam>
    /// <typeparam name="T7">此委托封装的方法的第七个参数的类型。</typeparam>
    /// <typeparam name="T8">此委托封装的方法的第八个参数的类型。</typeparam>
    /// <typeparam name="T9">此委托封装的方法的第九个参数的类型。</typeparam>
    /// <typeparam name="T10">此委托封装的方法的第十个参数的类型。</typeparam>
    /// <param name="arg1">此委托封装的方法的第一个参数。</param>
    /// <param name="arg2">此委托封装的方法的第二个参数。</param>
    /// <param name="arg3">此委托封装的方法的第三个参数。</param>
    /// <param name="arg4">此委托封装的方法的第四个参数。</param>
    /// <param name="arg5">此委托封装的方法的第五个参数。</param>
    /// <param name="arg6">此委托封装的方法的第六个参数。</param>
    /// <param name="arg7">此委托封装的方法的第七个参数。</param>
    /// <param name="arg8">此委托封装的方法的第八个参数。</param>
    /// <param name="arg9">此委托封装的方法的第九个参数。</param>
    /// <param name="arg10">此委托封装的方法的第十个参数。</param>
    public delegate void GameFrameworkAction<in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8, in T9, in T10>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10);

    /// <summary>
    /// 封装一个方法，该方法具有十一个参数并且不返回值。
    /// </summary>
    /// <typeparam name="T1">此委托封装的方法的第一个参数的类型。</typeparam>
    /// <typeparam name="T2">此委托封装的方法的第二个参数的类型。</typeparam>
    /// <typeparam name="T3">此委托封装的方法的第三个参数的类型。</typeparam>
    /// <typeparam name="T4">此委托封装的方法的第四个参数的类型。</typeparam>
    /// <typeparam name="T5">此委托封装的方法的第五个参数的类型。</typeparam>
    /// <typeparam name="T6">此委托封装的方法的第六个参数的类型。</typeparam>
    /// <typeparam name="T7">此委托封装的方法的第七个参数的类型。</typeparam>
    /// <typeparam name="T8">此委托封装的方法的第八个参数的类型。</typeparam>
    /// <typeparam name="T9">此委托封装的方法的第九个参数的类型。</typeparam>
    /// <typeparam name="T10">此委托封装的方法的第十个参数的类型。</typeparam>
    /// <typeparam name="T11">此委托封装的方法的第十一个参数的类型。</typeparam>
    /// <param name="arg1">此委托封装的方法的第一个参数。</param>
    /// <param name="arg2">此委托封装的方法的第二个参数。</param>
    /// <param name="arg3">此委托封装的方法的第三个参数。</param>
    /// <param name="arg4">此委托封装的方法的第四个参数。</param>
    /// <param name="arg5">此委托封装的方法的第五个参数。</param>
    /// <param name="arg6">此委托封装的方法的第六个参数。</param>
    /// <param name="arg7">此委托封装的方法的第七个参数。</param>
    /// <param name="arg8">此委托封装的方法的第八个参数。</param>
    /// <param name="arg9">此委托封装的方法的第九个参数。</param>
    /// <param name="arg10">此委托封装的方法的第十个参数。</param>
    /// <param name="arg11">此委托封装的方法的第十一个参数。</param>
    public delegate void GameFrameworkAction<in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8, in T9, in T10, in T11>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11);

    /// <summary>
    /// 封装一个方法，该方法具有十二个参数并且不返回值。
    /// </summary>
    /// <typeparam name="T1">此委托封装的方法的第一个参数的类型。</typeparam>
    /// <typeparam name="T2">此委托封装的方法的第二个参数的类型。</typeparam>
    /// <typeparam name="T3">此委托封装的方法的第三个参数的类型。</typeparam>
    /// <typeparam name="T4">此委托封装的方法的第四个参数的类型。</typeparam>
    /// <typeparam name="T5">此委托封装的方法的第五个参数的类型。</typeparam>
    /// <typeparam name="T6">此委托封装的方法的第六个参数的类型。</typeparam>
    /// <typeparam name="T7">此委托封装的方法的第七个参数的类型。</typeparam>
    /// <typeparam name="T8">此委托封装的方法的第八个参数的类型。</typeparam>
    /// <typeparam name="T9">此委托封装的方法的第九个参数的类型。</typeparam>
    /// <typeparam name="T10">此委托封装的方法的第十个参数的类型。</typeparam>
    /// <typeparam name="T11">此委托封装的方法的第十一个参数的类型。</typeparam>
    /// <typeparam name="T12">此委托封装的方法的第十二个参数的类型。</typeparam>
    /// <param name="arg1">此委托封装的方法的第一个参数。</param>
    /// <param name="arg2">此委托封装的方法的第二个参数。</param>
    /// <param name="arg3">此委托封装的方法的第三个参数。</param>
    /// <param name="arg4">此委托封装的方法的第四个参数。</param>
    /// <param name="arg5">此委托封装的方法的第五个参数。</param>
    /// <param name="arg6">此委托封装的方法的第六个参数。</param>
    /// <param name="arg7">此委托封装的方法的第七个参数。</param>
    /// <param name="arg8">此委托封装的方法的第八个参数。</param>
    /// <param name="arg9">此委托封装的方法的第九个参数。</param>
    /// <param name="arg10">此委托封装的方法的第十个参数。</param>
    /// <param name="arg11">此委托封装的方法的第十一个参数。</param>
    /// <param name="arg12">此委托封装的方法的第十二个参数。</param>
    public delegate void GameFrameworkAction<in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8, in T9, in T10, in T11, in T12>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12);

    /// <summary>
    /// 封装一个方法，该方法具有十三个参数并且不返回值。
    /// </summary>
    /// <typeparam name="T1">此委托封装的方法的第一个参数的类型。</typeparam>
    /// <typeparam name="T2">此委托封装的方法的第二个参数的类型。</typeparam>
    /// <typeparam name="T3">此委托封装的方法的第三个参数的类型。</typeparam>
    /// <typeparam name="T4">此委托封装的方法的第四个参数的类型。</typeparam>
    /// <typeparam name="T5">此委托封装的方法的第五个参数的类型。</typeparam>
    /// <typeparam name="T6">此委托封装的方法的第六个参数的类型。</typeparam>
    /// <typeparam name="T7">此委托封装的方法的第七个参数的类型。</typeparam>
    /// <typeparam name="T8">此委托封装的方法的第八个参数的类型。</typeparam>
    /// <typeparam name="T9">此委托封装的方法的第九个参数的类型。</typeparam>
    /// <typeparam name="T10">此委托封装的方法的第十个参数的类型。</typeparam>
    /// <typeparam name="T11">此委托封装的方法的第十一个参数的类型。</typeparam>
    /// <typeparam name="T12">此委托封装的方法的第十二个参数的类型。</typeparam>
    /// <typeparam name="T13">此委托封装的方法的第十三个参数的类型。</typeparam>
    /// <param name="arg1">此委托封装的方法的第一个参数。</param>
    /// <param name="arg2">此委托封装的方法的第二个参数。</param>
    /// <param name="arg3">此委托封装的方法的第三个参数。</param>
    /// <param name="arg4">此委托封装的方法的第四个参数。</param>
    /// <param name="arg5">此委托封装的方法的第五个参数。</param>
    /// <param name="arg6">此委托封装的方法的第六个参数。</param>
    /// <param name="arg7">此委托封装的方法的第七个参数。</param>
    /// <param name="arg8">此委托封装的方法的第八个参数。</param>
    /// <param name="arg9">此委托封装的方法的第九个参数。</param>
    /// <param name="arg10">此委托封装的方法的第十个参数。</param>
    /// <param name="arg11">此委托封装的方法的第十一个参数。</param>
    /// <param name="arg12">此委托封装的方法的第十二个参数。</param>
    /// <param name="arg13">此委托封装的方法的第十三个参数。</param>
    public delegate void GameFrameworkAction<in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8, in T9, in T10, in T11, in T12, in T13>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13);

    /// <summary>
    /// 封装一个方法，该方法具有十四个参数并且不返回值。
    /// </summary>
    /// <typeparam name="T1">此委托封装的方法的第一个参数的类型。</typeparam>
    /// <typeparam name="T2">此委托封装的方法的第二个参数的类型。</typeparam>
    /// <typeparam name="T3">此委托封装的方法的第三个参数的类型。</typeparam>
    /// <typeparam name="T4">此委托封装的方法的第四个参数的类型。</typeparam>
    /// <typeparam name="T5">此委托封装的方法的第五个参数的类型。</typeparam>
    /// <typeparam name="T6">此委托封装的方法的第六个参数的类型。</typeparam>
    /// <typeparam name="T7">此委托封装的方法的第七个参数的类型。</typeparam>
    /// <typeparam name="T8">此委托封装的方法的第八个参数的类型。</typeparam>
    /// <typeparam name="T9">此委托封装的方法的第九个参数的类型。</typeparam>
    /// <typeparam name="T10">此委托封装的方法的第十个参数的类型。</typeparam>
    /// <typeparam name="T11">此委托封装的方法的第十一个参数的类型。</typeparam>
    /// <typeparam name="T12">此委托封装的方法的第十二个参数的类型。</typeparam>
    /// <typeparam name="T13">此委托封装的方法的第十三个参数的类型。</typeparam>
    /// <typeparam name="T14">此委托封装的方法的第十四个参数的类型。</typeparam>
    /// <param name="arg1">此委托封装的方法的第一个参数。</param>
    /// <param name="arg2">此委托封装的方法的第二个参数。</param>
    /// <param name="arg3">此委托封装的方法的第三个参数。</param>
    /// <param name="arg4">此委托封装的方法的第四个参数。</param>
    /// <param name="arg5">此委托封装的方法的第五个参数。</param>
    /// <param name="arg6">此委托封装的方法的第六个参数。</param>
    /// <param name="arg7">此委托封装的方法的第七个参数。</param>
    /// <param name="arg8">此委托封装的方法的第八个参数。</param>
    /// <param name="arg9">此委托封装的方法的第九个参数。</param>
    /// <param name="arg10">此委托封装的方法的第十个参数。</param>
    /// <param name="arg11">此委托封装的方法的第十一个参数。</param>
    /// <param name="arg12">此委托封装的方法的第十二个参数。</param>
    /// <param name="arg13">此委托封装的方法的第十三个参数。</param>
    /// <param name="arg14">此委托封装的方法的第十四个参数。</param>
    public delegate void GameFrameworkAction<in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8, in T9, in T10, in T11, in T12, in T13, in T14>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14);

    /// <summary>
    /// 封装一个方法，该方法具有十五个参数并且不返回值。
    /// </summary>
    /// <typeparam name="T1">此委托封装的方法的第一个参数的类型。</typeparam>
    /// <typeparam name="T2">此委托封装的方法的第二个参数的类型。</typeparam>
    /// <typeparam name="T3">此委托封装的方法的第三个参数的类型。</typeparam>
    /// <typeparam name="T4">此委托封装的方法的第四个参数的类型。</typeparam>
    /// <typeparam name="T5">此委托封装的方法的第五个参数的类型。</typeparam>
    /// <typeparam name="T6">此委托封装的方法的第六个参数的类型。</typeparam>
    /// <typeparam name="T7">此委托封装的方法的第七个参数的类型。</typeparam>
    /// <typeparam name="T8">此委托封装的方法的第八个参数的类型。</typeparam>
    /// <typeparam name="T9">此委托封装的方法的第九个参数的类型。</typeparam>
    /// <typeparam name="T10">此委托封装的方法的第十个参数的类型。</typeparam>
    /// <typeparam name="T11">此委托封装的方法的第十一个参数的类型。</typeparam>
    /// <typeparam name="T12">此委托封装的方法的第十二个参数的类型。</typeparam>
    /// <typeparam name="T13">此委托封装的方法的第十三个参数的类型。</typeparam>
    /// <typeparam name="T14">此委托封装的方法的第十四个参数的类型。</typeparam>
    /// <typeparam name="T15">此委托封装的方法的第十五个参数的类型。</typeparam>
    /// <param name="arg1">此委托封装的方法的第一个参数。</param>
    /// <param name="arg2">此委托封装的方法的第二个参数。</param>
    /// <param name="arg3">此委托封装的方法的第三个参数。</param>
    /// <param name="arg4">此委托封装的方法的第四个参数。</param>
    /// <param name="arg5">此委托封装的方法的第五个参数。</param>
    /// <param name="arg6">此委托封装的方法的第六个参数。</param>
    /// <param name="arg7">此委托封装的方法的第七个参数。</param>
    /// <param name="arg8">此委托封装的方法的第八个参数。</param>
    /// <param name="arg9">此委托封装的方法的第九个参数。</param>
    /// <param name="arg10">此委托封装的方法的第十个参数。</param>
    /// <param name="arg11">此委托封装的方法的第十一个参数。</param>
    /// <param name="arg12">此委托封装的方法的第十二个参数。</param>
    /// <param name="arg13">此委托封装的方法的第十三个参数。</param>
    /// <param name="arg14">此委托封装的方法的第十四个参数。</param>
    /// <param name="arg15">此委托封装的方法的第十五个参数。</param>
    public delegate void GameFrameworkAction<in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8, in T9, in T10, in T11, in T12, in T13, in T14, in T15>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15);

    /// <summary>
    /// 封装一个方法，该方法具有十六个参数并且不返回值。
    /// </summary>
    /// <typeparam name="T1">此委托封装的方法的第一个参数的类型。</typeparam>
    /// <typeparam name="T2">此委托封装的方法的第二个参数的类型。</typeparam>
    /// <typeparam name="T3">此委托封装的方法的第三个参数的类型。</typeparam>
    /// <typeparam name="T4">此委托封装的方法的第四个参数的类型。</typeparam>
    /// <typeparam name="T5">此委托封装的方法的第五个参数的类型。</typeparam>
    /// <typeparam name="T6">此委托封装的方法的第六个参数的类型。</typeparam>
    /// <typeparam name="T7">此委托封装的方法的第七个参数的类型。</typeparam>
    /// <typeparam name="T8">此委托封装的方法的第八个参数的类型。</typeparam>
    /// <typeparam name="T9">此委托封装的方法的第九个参数的类型。</typeparam>
    /// <typeparam name="T10">此委托封装的方法的第十个参数的类型。</typeparam>
    /// <typeparam name="T11">此委托封装的方法的第十一个参数的类型。</typeparam>
    /// <typeparam name="T12">此委托封装的方法的第十二个参数的类型。</typeparam>
    /// <typeparam name="T13">此委托封装的方法的第十三个参数的类型。</typeparam>
    /// <typeparam name="T14">此委托封装的方法的第十四个参数的类型。</typeparam>
    /// <typeparam name="T15">此委托封装的方法的第十五个参数的类型。</typeparam>
    /// <typeparam name="T16">此委托封装的方法的第十六个参数的类型。</typeparam>
    /// <param name="arg1">此委托封装的方法的第一个参数。</param>
    /// <param name="arg2">此委托封装的方法的第二个参数。</param>
    /// <param name="arg3">此委托封装的方法的第三个参数。</param>
    /// <param name="arg4">此委托封装的方法的第四个参数。</param>
    /// <param name="arg5">此委托封装的方法的第五个参数。</param>
    /// <param name="arg6">此委托封装的方法的第六个参数。</param>
    /// <param name="arg7">此委托封装的方法的第七个参数。</param>
    /// <param name="arg8">此委托封装的方法的第八个参数。</param>
    /// <param name="arg9">此委托封装的方法的第九个参数。</param>
    /// <param name="arg10">此委托封装的方法的第十个参数。</param>
    /// <param name="arg11">此委托封装的方法的第十一个参数。</param>
    /// <param name="arg12">此委托封装的方法的第十二个参数。</param>
    /// <param name="arg13">此委托封装的方法的第十三个参数。</param>
    /// <param name="arg14">此委托封装的方法的第十四个参数。</param>
    /// <param name="arg15">此委托封装的方法的第十五个参数。</param>
    /// <param name="arg16">此委托封装的方法的第十六个参数。</param>
    public delegate void GameFrameworkAction<in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8, in T9, in T10, in T11, in T12, in T13, in T14, in T15, in T16>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, T16 arg16);
}
