﻿using UnityEngine;

namespace DimensionCollapse
{
    public class RPCManager : Photon.PunBehaviour
    {
        private PlayerManager playerManager;
        #region private add by ZQF
        //拾取脚本
        private PickupManager pickupManager;
        #endregion
        private void Start()
        {
            playerManager = GetComponent<PlayerManager>();

            #region add by ZQF
            pickupManager = GetComponent<PickupManager>();
            #endregion
        }

        public void UseItemInHandRPC()
        {
            if (photonView.isMine)
            {
                photonView.RPC("UseItemInHand", PhotonTargets.All);
            }
        }

        public void CastSkillOneRPC()
        {
            if (photonView.isMine)
            {
                photonView.RPC("CastSkillOne", PhotonTargets.All);
            }
        }

        public void CastSkillTwoRPC()
        {
            if (photonView.isMine)
            {
                photonView.RPC("CastSkillTwo", PhotonTargets.All);
            }
        }
        
        public void PickUpItemRPC()
        {
            if (photonView.isMine)
            {
                photonView.RPC("PickUpItem", PhotonTargets.All);
            }
        }

        public void DropHandItemRPC()
        {
            if (photonView.isMine)
            {
                photonView.RPC("DropHandItem", PhotonTargets.All);
            }
        }

        [PunRPC]
        private void UseItemInHand()
        {
            Item item = playerManager.itemInHand;
            (item as Weapon)?.Attack();
        }

        [PunRPC]
        private void CastSkillOne()
        {
            CastSkill(playerManager.skillOne);
        }

        [PunRPC]
        private void CastSkillTwo()
        {
            CastSkill(playerManager.skillTwo);
        }

        private void CastSkill(Skill skill)
        {
            (skill as NondirectiveSkill).Cast();
        }

        #region function add by ZQF
        [PunRPC]
        private void PickUpItem()
        {
            pickupManager.PickItem();
        }
        [PunRPC]
        private void DropHandItem()
        {
            pickupManager.DropHandItem();
        }
        #endregion

    }
}