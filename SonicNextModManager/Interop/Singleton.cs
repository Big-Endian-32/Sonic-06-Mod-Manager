namespace SonicNextModManager.Interop
{
    public struct Singleton<T>
    {
        private static T? StaticInstance { get; set; }

        public T Instance => StaticInstance;

        public Singleton(T in_instance)
            => SetInstance(in_instance);

        public static void SetInstance(T in_instance)
            => StaticInstance = in_instance;

        public static T GetInstance()
            => StaticInstance;

        public static bool HasInstance()
            => StaticInstance != null;

        public static implicit operator T(Singleton<T> in_singleton)
            => GetInstance();
    }

    public struct Singleton
    {
        public static T GetInstance<T>()
            => Singleton<T>.GetInstance();

        public static void SetInstance<T>(T in_instance)
            => Singleton<T>.SetInstance(in_instance);

        public static bool HasInstance<T>()
            => Singleton<T>.HasInstance();
    }
}
