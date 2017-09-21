using System.Collections.Generic;


internal class XBulletMgr : XSingleton<XBulletMgr>
{
    private List<XBullet> _bullets = new List<XBullet>();

    public void ShootBullet(XBullet bullet)
    {
        _bullets.Add(bullet);
    }

    public void Update(float fDeltaT)
    {
        int len = _bullets.Count;

        for (int i = len - 1; i >= 0; i--)
        {
            if (_bullets[i].IsExpired())
            {
                _bullets[i].Destroy();
                _bullets.RemoveAt(i);
            }
            else
                _bullets[i].Update(fDeltaT);
        }
    }
}