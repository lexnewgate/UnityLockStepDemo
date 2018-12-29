using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


/// <summary>
/// 由于是模拟帧同步,做如下假定:
/// 当使用GeneralAction时,我们假定每次收发一定成功.
/// </summary>
public interface IGeneralAction
{
    void Handle(IVirtualClient virtualClient);
}
