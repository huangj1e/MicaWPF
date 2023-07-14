﻿// <copyright file="PtrHelper.cs" company="Zircon Technology">
// This software is distributed under the MIT license and its code is open-source and free for use, modification, and distribution.
// </copyright>

using System.Runtime.CompilerServices;

namespace MicaWPF.Core.Helpers;

#if NET7_0_OR_GREATER
public static class PtrHelper
{
    public static nint Zero { get; } = nint.Zero;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static nint Create(int value)
    {
        return new nint(value);
    }
}
#else
public static class PtrHelper
{
    public static IntPtr Zero { get; } = IntPtr.Zero;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IntPtr Create(int value)
    {
        return new IntPtr(value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int ToInt32(this nint nintPtr)
    {
        var ptr = (IntPtr)nintPtr;
        return ptr.ToInt32();
    }
}

#endif
