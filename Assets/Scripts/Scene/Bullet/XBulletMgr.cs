using System.Collections.Generic;


internal class XBulletMgr : XSingleton<XBulletMgr>
{
    private List<XBullet> _bullets = new List<XBullet>();
    private int _len=0;

    public void ShootBullet(XBullet bullet)
    {
        _bullets.Add(bullet);
        _len = _bullets.Count;
    }

    public void Update(float fDeltaT)
    {
        for (int i = _len - 1; i >= 0; i--)
        {
            if (_bullets[i].IsExpired())
            {
                _bullets[i].Destroy();
                _bullets.RemoveAt(i);
                _len--;
            }
            else
            {
                _bullets[i].Update(fDeltaT);
            }
        }
    }

}