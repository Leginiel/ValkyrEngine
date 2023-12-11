using System.Runtime.CompilerServices;

namespace ValkyrEngine.Rendering.Extensions;

public static class EnumFlagExtensions
{
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static void SetFlag<T>(this ref T @enum, T flag) where T : unmanaged, Enum
  {
    switch (Unsafe.SizeOf<T>()) // match default enum size first
    {
      case 4:
        Unsafe.As<T, uint>(ref @enum) |= Unsafe.As<T, uint>(ref flag);
        break;
      case 8:
        Unsafe.As<T, ulong>(ref @enum) |= Unsafe.As<T, ulong>(ref flag);
        break;
      case 1:
        Unsafe.As<T, byte>(ref @enum) |= Unsafe.As<T, byte>(ref flag);
        break;
      case 2:
        Unsafe.As<T, short>(ref @enum) |= Unsafe.As<T, short>(ref flag);
        break;
    }
  }
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static bool HasFlag<T>(this ref T @enum, T flag) where T : unmanaged, Enum
  {
    return Unsafe.SizeOf<T>() switch // match default enum size first
    {
      4 => (Unsafe.As<T, uint>(ref @enum) & Unsafe.As<T, uint>(ref flag)) != Unsafe.As<T, uint>(ref flag),
      8 => (Unsafe.As<T, ulong>(ref @enum) & Unsafe.As<T, ulong>(ref flag)) != Unsafe.As<T, ulong>(ref flag),
      1 => (Unsafe.As<T, byte>(ref @enum) & Unsafe.As<T, byte>(ref flag)) != Unsafe.As<T, byte>(ref flag),
      2 => (Unsafe.As<T, short>(ref @enum) & Unsafe.As<T, short>(ref flag)) != Unsafe.As<T, short>(ref flag),
      _ => false,
    };
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static void ClearFlag<T>(this ref T @enum, T flag) where T : unmanaged, Enum
  {
    switch (Unsafe.SizeOf<T>()) // match default enum size first
    {
      case 4:
        Unsafe.As<T, uint>(ref @enum) &= ~Unsafe.As<T, uint>(ref flag);
        break;
      case 8:
        Unsafe.As<T, ulong>(ref @enum) &= ~Unsafe.As<T, ulong>(ref flag);
        break;
      case 1:
        Unsafe.As<T, byte>(ref @enum) &= (byte)~Unsafe.As<T, byte>(ref flag);
        break;
      case 2:
        Unsafe.As<T, short>(ref @enum) &= (short)~Unsafe.As<T, short>(ref flag);
        break;
    }
  }
}