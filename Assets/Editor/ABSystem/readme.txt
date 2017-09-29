AB系统：

1.  自己维护实现依赖关系 没有使用U5的依赖

2.  处理依赖关系：递归树（先加载依赖->再加载本身）
	对于有依赖的prefab，先把对应的依赖的资源（如Texture）先load进去，然后Load Prefab。 这样prefab上绑定(依赖)的资源会自动对应到prefab
	当然，打ab的时候要先将prefab里依赖的资源解开，否则会导致ab过大

3.  自己实现XAssetBundle U5加载出来的bundle不能立即卸载 比说一个texture对应的bundle unload之后，texture也会被卸载 要等待一段时间后再去卸载
	公共依赖的资源最好不要卸载，因为有依赖的资源需要加载的时候bundle需要再次load，公共资源的ab卸载选择恰当的时机，比如说切场景
	因为公共依赖的资源不卸载，会造成内存的升高，所以在设计的时候，尽量避免依赖。
	或者再打ab的时候，手动的解依赖，运行时自动的维护依赖关系，而不使用unity自带的依赖维护

4.  针对Asset-Object 而非Cloned-Object的卸载（bundle.loadasset出来的对象，而非Instantiate）
	如果对象是GameObject 在编辑器模式下无法卸载go物体，否则会报错让改用DestroyImmediate(obj, true) 这样做会连文件夹里的原始Asset一并删除
	如果对象是Texture、Material等使用Resources.UnloadAsset后, 若依然有物体用该图，那么物体就变全黑
 
5.  建议美术工程和程序工程分开。程序工程没有原始的资源，都是二进制，包括avatar,ui, scene等。美术工程有原始资源转换ab的工具，程序工程这样出包的时候就不用每次都打ab 
	资源ab由美术维护

6.  Object类重载了类型隐式转换运算符== 这个在与null比较时要特别注意，既使是有效的引用结果有可能是true的
	GameObject go = new GameObject(); 
	Debug.Log (go == null); // false

	Object obj = new Object();
	Debug.Log (obj == null); // true

	Instatiating a GameObject adds it to the scene so it is completely initialized (!destroyed). Instantiating a simple UnityEngine.Object has no such semantics, so the it stays in the 'destroyed' state which compares true to null.

