using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;
using Unity.Mathematics;
using UnityEngine;
using Unity.Collections;
using Unity.Physics.Systems;
using Unity.Physics;

[UpdateAfter(typeof(TempEntityRotateSystem))]
public class SwordRotateSystem : SystemBase
{

    EndSimulationEntityCommandBufferSystem m_EndSimulationEcbSystem;

    protected override void OnCreate()
    {
        base.OnCreate();

        // ��World�л�ȡECSϵͳ���Ҵ�����
        m_EndSimulationEcbSystem = World
            .GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();

    }
    protected override void OnUpdate()
    {
        bool isGo = false;
        float3 hitpos = float3.zero;
        float deltaTime = Time.DeltaTime;

        // ����һ��ECS����ת���ɿɲ��е�
        var ecb = m_EndSimulationEcbSystem.CreateCommandBuffer().AsParallelWriter();
        if (Input.GetMouseButtonDown(0))
        {
            //��ȡ��������
            BuildPhysicsWorld physicsWorld = World.DefaultGameObjectInjectionWorld.GetExistingSystem<BuildPhysicsWorld>();
            NativeArray<RigidBody> rigidBodies = new NativeArray<RigidBody>(1, Allocator.TempJob);
            NativeArray<float3> rayHitPos = new NativeArray<float3>(1, Allocator.TempJob);
            //��ȡ���߷���λ��
            UnityEngine.Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastJobHandle raycastJonHande = new RaycastJobHandle()
            {
                mStartPos = ray.origin,
                mEndPos = ray.direction * 10000,
                physicsWorld = physicsWorld.PhysicsWorld,
                Bodies = rigidBodies,
                rayHitpos = rayHitPos
            };

            //��Ҫ������ǰJob
            JobHandle jobHandle = raycastJonHande.Schedule(this.Dependency);
            jobHandle.Complete();

            if (rigidBodies[0].Entity != null)
            {
                Debug.Log("Ŀ�����꣺" + rayHitPos[0]);
                Debug.Log("���߻���Ŀ��" + rigidBodies[0].Entity);
                hitpos = rayHitPos[0];
                isGo = true;
            }

            rigidBodies.Dispose();
            rayHitPos.Dispose();

        }

        Entities.
         WithAll<SwordTag>().
         WithNone<GoTag>().
         ForEach((Entity entity, int entityInQueryIndex, ref Translation translation, ref Rotation orientation, ref Target target) =>
         {
             #region �ɽ�Ⱥ������
             if (isGo && entityInQueryIndex < 10000)
             {

                 GoTag tag = new GoTag
                 {
                     targetPos = hitpos,
                     TempEntity = target.targetTempentity,
                     originPos = translation.Value,
                     isBack = false
                 };

                 // ��entityInQueryIndex��������������ECS�ط�ʱ�ܱ�֤��ȷ��˳��
                 ecb.AddComponent(entityInQueryIndex, entity, tag);
             }
             #endregion

             if (!HasComponent<LocalToWorld>(target.targetTempentity))
             {
                 return;
             }

             var rotation = orientation;

             float3 targetPosition = target.Tpos;

             var targetDir = targetPosition - translation.Value;

             //�ɽ���ֱ�����������ĵ�
             quaternion temp1 = Quaternion.FromToRotation(Vector3.left, targetDir);

             orientation.Value = temp1;

             LocalToWorld tempEntityPos = GetComponent<LocalToWorld>(target.targetTempentity);
             translation.Value = tempEntityPos.Position;

         }).ScheduleParallel();

        // ��֤ECB system������ǰ���Job
        m_EndSimulationEcbSystem.AddJobHandleForProducer(this.Dependency);
    }

    //��������Job
    public struct RaycastJobHandle : IJob
    {

        public NativeArray<RigidBody> Bodies;
        public NativeArray<float3> rayHitpos;
        public float3 mStartPos;
        public float3 mEndPos;
        public PhysicsWorld physicsWorld;

        public void Execute()
        {
            //��������
            RaycastInput raycastInput = new RaycastInput()
            {
                Start = mStartPos,
                End = mEndPos * 100,
                //������ײ����������������ĳЩ�㼶�µ������Ƿ�������߼��
                Filter = new CollisionFilter() { BelongsTo = ~0u, CollidesWith = ~0u, GroupIndex = 0, }
            };
            Unity.Physics.RaycastHit raycastHit = new Unity.Physics.RaycastHit();

            // ��������ȥ���Entityʵ��
            if (physicsWorld.CollisionWorld.CastRay(raycastInput, out raycastHit))
            {
                //�õ��������߻��е�entity
                Bodies[0] = physicsWorld.Bodies[raycastHit.RigidBodyIndex];
                //�õ����е��λ����Ϣ
                rayHitpos[0] = raycastHit.Position;
            }
        }
    }
}

