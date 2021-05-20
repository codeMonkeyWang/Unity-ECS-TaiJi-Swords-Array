using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;
using Unity.Mathematics;
public struct TempEntityTag : IComponentData{}
public class TempEntityRotateSystem : SystemBase
{
    protected override void OnUpdate()
    {
        float deltaTime =Time.DeltaTime;
        float angel = 0.01f;

        Entities.
            WithAll<RotateTag,TempEntityTag>().
            ForEach(( ref Translation translation, in Target target) =>
        {

            #region ������TempEntity Χ�ƽ������ĵ���ת

            float3 pos = translation.Value;
            //��ת�����ת�Ƕ�
            quaternion rot = quaternion.AxisAngle(math.up(), angel);
            float3 dir = pos - target.Tpos;

            dir = math.mul(rot, dir);

            translation.Value = target.Tpos + dir;

            #endregion

        }).ScheduleParallel();

    }

}
