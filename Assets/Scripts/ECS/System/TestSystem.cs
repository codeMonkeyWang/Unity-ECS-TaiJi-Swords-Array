/*
 *����ǳ��Systembase  https://zhuanlan.zhihu.com/p/252858463
 */

using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;
using Unity.Mathematics;
using UnityEngine;



[AlwaysSynchronizeSystem]
public class TestSystem : SystemBase
{

    //�Ѿ�������
    protected override void OnUpdate()
    {
        float deltaTime = this.Time.DeltaTime;
        float angel =0.01f;
        Entities.ForEach((ref Rotation orientation, ref Translation position,
            in LocalToWorld transform,
            in Target target) =>
            {

                // Check to make sure the target Entity still exists and has
                // the needed component
                //if (!HasComponent<LocalToWorld>(target.entity))
                //{
                //   // Debug.Log("Noit Find Target");
                //    return;
                //}

                #region ����Target����
                //return;
                // Look up the entity data
                //LocalToWorld targetTransform
                //    = GetComponent<LocalToWorld>(target.entity);


                //float3 targetPosition = targetTransform.Position;

                //// Calculate the rotation
                //float3 displacement = targetPosition - transform.Position;

                ////float3 upReference = new float3(0, 1, 0);
                //float3 upReference = new float3(0, -1, 1);
                //quaternion lookRotation =
                //    quaternion.LookRotationSafe(displacement, upReference);

                //orientation.Value =
                //    math.slerp(orientation.Value, lookRotation, deltaTime);

                #region �̶����泯�����ȥ�İ취

                var rotation = orientation;
                //LocalToWorld targetTransform
                //    = GetComponent<LocalToWorld>(target.entity);


                //float3 targetPosition = targetTransform.Position;
                //var targetDir = targetPosition - position.Value;


                //quaternion temp1 = Quaternion.FromToRotation(Vector3.down, targetDir);

                //orientation.Value = math.slerp(orientation.Value, temp1, deltaTime);

                #endregion





                #endregion

                #region Χ��Target��ת


                //float3 pos = position.Value;
                ////��ת�����ת�Ƕ�
                //quaternion rot = quaternion.AxisAngle(math.up(), angel);
                //float3 dir = pos - targetTransform.Position;

                //dir = math.mul(rot, dir);
                ////
                //position.Value = targetTransform.Position + dir;
                //// position.Value = math.lerp(position.Value, targetTransform.Position + dir,deltaTime);//�ƶ�����λ��
                //var myrot = orientation.Value;
                //orientation.Value = math.mul(rot, myrot);



                #endregion


            }).ScheduleParallel();
            
    }


    //���еĺ��������ǲ������

    /// <summary>
    /// ��ĳ����ȥ��������
    /// </summary>
    /// <param name="tr_self">����ı���</param>
    /// <param name="lookPos">�����Ŀ��</param>
    /// <param name="directionAxis">�����ᣬȡ���������Ǹ�����ȥ����</param>
    public quaternion AxisLookAt(Rotation rot, Translation pos, float3 lookPos, float3 directionAxis)
    {
        var rotation = rot;
        var targetDir = lookPos - pos.Value;
        //ָ���ĸ��ᳯ��Ŀ��,�����޸�Vector3�ķ���
        var fromDir = math.mul(rotation.Value, directionAxis);
        //���㴹ֱ�ڵ�ǰ�����Ŀ�귽�����

        var axis = math.cross(fromDir, targetDir);
        axis = math.normalize(axis);
        // var axis = Vector3.Cross(fromDir, targetDir).normalized;

        //���㵱ǰ�����Ŀ�귽��ļн�
        //var angle = Vector3.Angle(fromDir, targetDir);
        var angle2 = math.dot(fromDir, targetDir);
        //����ǰ������Ŀ�귽����תһ���Ƕȣ�����Ƕ�ֵ��������ֵ


        quaternion temp = math.mul(quaternion.AxisAngle(axis, angle2), rotation.Value);
        return temp;
        // tr_self.rotation = Quaternion.AngleAxis(angle, axis) * rotation.Value;
        // tr_self.localEulerAngles = new Vector3(0, tr_self.localEulerAngles.y, 90);//�����������ӵģ���Ϊ������x��z���򲻻����κα仯

    }
}