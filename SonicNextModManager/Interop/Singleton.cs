namespace SonicNextModManager.Interop
{
    public struct Singleton<T>
    {
        private static T? StaticInstance { get; set; }

        public T Instance => StaticInstance;

        public Singleton(T instance)
            => SetInstance(instance);

        public static void SetInstance(T instance)
            => StaticInstance = instance;

        public static T GetInstance()
            => StaticInstance;

        public static bool HasInstance()
            => StaticInstance != null;

        public static implicit operator T(Singleton<T> singleton)
            => GetInstance();
    }

    public struct Singleton
    {
        public static T GetInstance<T>()
            => Singleton<T>.GetInstance();

        public static void SetInstance<T>(T instance)
            => Singleton<T>.SetInstance(instance);

        public static bool HasInstance<T>()
            => Singleton<T>.HasInstance();
    }
}
