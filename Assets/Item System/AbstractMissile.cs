﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace DimensionCollapse
{
    public abstract class AbstractMissile : Missile
    {
        private float maxChargeTime;
        private float maxForce;
        private new Camera camera;
        public float timeBeforeExplode = 5f;
        public float timeBeforeDisappear = 3f;
        public AudioSource explodeSoundEffect;
        public ParticleSystem explodeViewEffect;
        [ReadOnlyInInspector]
        public float chargeTime;

        private bool isCharging;

        public override void OnChargeStart()
        {
            PlayerManager playerManager = GetComponentInParent<PlayerManager>();
            maxChargeTime = playerManager.maxChargeTime;
            maxForce = playerManager.maxChargeForce;
            camera = playerManager.camera;
            chargeTime = 0f;
            isCharging = true;
        }

        public override void OnCharge()
        {
            if (isCharging)
            {
                chargeTime += Time.deltaTime;
                //if (chargeTime >= maxChargeTime)
                //{
                //    chargeTime = maxChargeTime;
                //    OnChargeEnd();
                //}
            }
        }

        public override Vector3 OnChargeEnd()
        {
            return camera.transform.forward.normalized * (chargeTime / maxChargeTime) * maxForce;
        }

        public override void Throw(Vector3 force)
        {
            isCharging = false;

            Collider collider = GetComponent<Collider>();
            if (collider != null)
            {
                collider.enabled = true;
            }

            InstantBeforeThrow();

            transform.parent = null;
            Rigidbody rigidbody = GetComponent<Rigidbody>();
            if (rigidbody == null)
            {
                rigidbody = gameObject.AddComponent<Rigidbody>();
            }
            rigidbody.constraints = RigidbodyConstraints.None;
            rigidbody.AddForce(force, ForceMode.Impulse);

            StartCoroutine(ExplodeCoroutine());
        }

        private IEnumerator ExplodeCoroutine()
        {
            yield return new WaitForSeconds(timeBeforeExplode - chargeTime);
            OnExplode();
            yield return new WaitForSeconds(timeBeforeDisappear);
            PhotonNetwork.Destroy(gameObject);
        }

        private void OnExplode()
        {
            Destroy(GetComponent<Rigidbody>());
            foreach (var renderer in GetComponentsInChildren<MeshRenderer>())
            {
                renderer.enabled = false;
            }
            GetComponent<Collider>().enabled = false;
            ItemUtils.Play(explodeSoundEffect);
            ItemUtils.Play(explodeViewEffect, true);
            ExplodeCore();
        }

        public virtual void InstantBeforeThrow()
        {

        }

        public abstract void ExplodeCore();

        public override void OnPickedUp(PlayerManager playerManager)
        {
            Collider collider = GetComponent<Collider>();
            if (collider != null)
            {
                collider.enabled = false;
            }
            Rigidbody[] rigidbodys = GetComponents<Rigidbody>();
            foreach (var rigidbody in rigidbodys)
            {
                ItemUtils.FreezeRigidbody(rigidbody);
            }
            Picked = true;
        }

        public override void OnThrown()
        {
            Collider collider = GetComponent<Collider>();
            if (collider != null)
            {
                collider.enabled = true;
            }
            ItemUtils.FreezeRigidbodyWithoutPositionY(GetComponent<Rigidbody>());
            Picked = false;
        }
    }
}
