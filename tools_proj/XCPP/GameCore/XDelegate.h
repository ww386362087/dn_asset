#ifndef  __XDelegate__
#define  __XDelegate__



class IArgs
{
public:
	virtual ~IArgs() {}
};

class XDelegate
{
public:
	XDelegate()
		: object_ptr(0)
		, stub_ptr(0)
	{}

	template <class T, bool (T::*TMethod)(IArgs*, void *)>
	static XDelegate registerMethod(T* object_ptr)
	{
		XDelegate d;
		d.object_ptr = object_ptr;
		d.stub_ptr = &method_stub<T, TMethod>; // #1
		return d;
	}

	bool operator()(IArgs *pargs, void *arg) const
	{
		if (object_ptr)
		{
			return stub_ptr(object_ptr, pargs, arg);
		}
		return false;
	}

private:
	typedef bool(*stub_type)(void* object_ptr, IArgs *, void *arg);

	void* object_ptr;
	stub_type stub_ptr;

	template <class T, bool (T::*TMethod)(IArgs *, void *)>
	static bool method_stub(void* object_ptr, IArgs *pargs, void *arg)
	{
		T* ptr = static_cast<T*>(object_ptr);
		//invocation here as fast as direct method invocation (because its value is known at compile time)
		return (ptr->*TMethod)(pargs, arg);
	}
};


#endif