#if defined(__CYGWIN32__)
#define ENGINE_INTERFACE_API __stdcall
#define ENGINE_INTERFACE_EXPORT __declspec(dllexport)
#elif defined(WIN32) || defined(_WIN32) || defined(__WIN32__) || defined(_WIN64) || defined(WINAPI_FAMILY)
#define ENGINE_INTERFACE_API __stdcall
#define ENGINE_INTERFACE_EXPORT __declspec(dllexport)
#elif defined(__MACH__) || defined(__ANDROID__) || defined(__linux__) || defined(__QNX__)
#define ENGINE_INTERFACE_API
#define ENGINE_INTERFACE_EXPORT
#else
#define ENGINE_INTERFACE_API
#define ENGINE_INTERFACE_EXPORT
#endif

