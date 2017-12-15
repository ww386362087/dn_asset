#ifndef _SINGLETON_H_
#define _SINGLETON_H_

// 非线程安全单例
template <typename T>
class Singleton
{
protected:
	Singleton() {}
	virtual ~Singleton() {}

public:
	static T* Instance()
	{
		if (m_instance == NULL)
		{
			m_instance = new T();
		}
		return m_instance;
	}

	static void DestroyInstance()
	{
		if (m_instance)
		{
			delete m_instance;
			m_instance = NULL;
		}
	}

protected:
	static T* m_instance;
};

template <typename T> T* Singleton<T>::m_instance = NULL;

#endif