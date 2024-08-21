namespace SonicNextModManager.Interop
{
    public struct Singleton<T>
    {
        private static T? StaticInstance { get; set; }

        public readonly T? Instance => StaticInstance;

        public Singleton(T in_instance)
        {
            SetInstance(in_instance);
        }

        public static void SetInstance(T in_instance)
        {
            StaticInstance = in_instance;
        }

        public static T? GetInstance()
        {
            return StaticInstance;
        }

        public static bool HasInstance()
        {
            return StaticInstance != null;
        }

        public static implicit operator T?(Singleton<T> in_singleton)
        {
            return GetInstance();
        }
    }

    public struct Singleton
    {
        public static T? GetInstance<T>()
        {
            return Singleton<T>.GetInstance();
        }

        public static void SetInstance<T>(T in_instance)
        {
            Singleton<T>.SetInstance(in_instance);
        }

        public static bool HasInstance<T>()
        {
            return Singleton<T>.HasInstance();
        }
    }
}
