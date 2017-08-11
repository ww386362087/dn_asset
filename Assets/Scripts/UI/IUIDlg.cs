using UnityEngine;

public interface IUIDlg
{

	uint id { get; }

    bool pushStack { get; }

    string fileName { get; }

    bool shareCanvas { get; }

    DlgBehaviourBase innerBehaviour { get; }

    DlgType type { get; }

    bool IsVisible();

    bool IsLoaded();

    void OnLoad();

    void OnShow();

    void OnHide();

    void OnDestroy();

    void SetVisible(bool visble);

    void SetBehaviour(GameObject _go);
}
