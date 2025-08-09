﻿using GorillaShirts.Behaviours.Appearance;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using GorillaNetworking;
using Photon.Pun;

#if PLUGIN
using GorillaShirts.Extensions;
using System;
using GorillaShirts.Models.Locations;
using System.Collections.Generic;
using System.Linq;
#endif

namespace GorillaShirts.Behaviours.UI
{
    public class Stand : MonoBehaviour
    {
        public GameObject Root;

        public AudioSource AudioDevice;

        public Camera Camera;

        public StandCharacterHumanoid Character;

        public GameObject interfaceRoot;

        [Header("Welcome Menu")]

        public GameObject welcomeMenuRoot;

        [Header("Load Menu")]

        public GameObject loadMenuRoot;

        public Image loadRadial;

        public TMP_Text loadPercent;

        public TMP_Text didYouKnowText;

        public TMP_Text flagText;

        public GameObject greenFlag;

        public GameObject redFlag;

        [Header("Version Notice Screen")]

        public GameObject versionMenuRoot;

        public TMP_Text versionDiffText;

        [TextArea(2, 4)]
        public string versionDiffFormat;

        public GameObject softVersionContainer, hardVersionContainer;

        [Header("Main Menu")]

        public GameObject mainMenuRoot;

        public GameObject navigationRoot;

        public GameObject mainContentRoot;

        public TMP_Text navigationText;

        public GameObject packBrowserNewSymbol;

        public TMP_Text headerText;

        [TextArea(2, 8)]
        public string headerFormat;

        public TMP_Text descriptionText;

        public Image previewImage;

        public GameObject[] featureObjects;

        public TMP_Text shirtStatusText;

        public Sidebar mainSideBar;

        public GameObject infoButtonObject;

        [Header("Main Menu (info)")]

        public GameObject infoContentRoot;

        public TMP_Text playerInfoText;

        [TextArea(4, 6), FormerlySerializedAs("personalDataFormat")]
        public string playerInfoFormat;

        [Header("Main Menu - Colour Manager")]

        public GameObject mainMenu_colourSubMenu;

        public TMP_Text colourPicker_NavText;

        public ColourPicker colourPicker;

        public GameObject colourPicker_SyncButton, colourPicker_ApplyButton;

        [Header("Pack Browser Load Menu")]

        public GameObject packBrowserMenuRoot;

        public Image packBrowserRadial;

        public TMP_Text packBrowserPercent;

        public TMP_Text packBrowserStatus;

        public TMP_Text packBrowserLabel;

        [Header("UNUSED FOR NOW")] // [Header("Main Menu (under Info tab)")]

        public GameObject tutorialRoot;

        public GameObject[] tutorialTabs;

        public GameObject statisticsRoot;

        public GameObject creditsRoot;

#if PLUGIN

        private readonly Dictionary<GTZone, Location_Base> locationFromZoneDict = [];

        private Renderer[] renderers;

        private Dictionary<Renderer, Material[]> materials;

        private Dictionary<Renderer, Material[]> uberMaterials;

        private bool uberMaterialsUsed = false;

        private bool isInMines = false;

        public void Start()
        {
            Type baseType = typeof(Location_Base);
            Type[] typeArray = baseType.Assembly.GetTypes();

            foreach (Type type in typeArray)
            {
                if (type.IsSubclassOf(baseType))
                {
                    Location_Base location = (Location_Base)Activator.CreateInstance(type);
                    location.Zones.Where(zone => !locationFromZoneDict.ContainsKey(zone)).ForEach(zone => locationFromZoneDict.Add(zone, location));
                }
            }

            renderers = Root.GetComponentsInChildren<Renderer>(true);

            materials = renderers.ToDictionary(renderer => renderer, renderer => renderer.materials);
            uberMaterials = renderers.ToDictionary(renderer => renderer, renderer => renderer.materials.Select(material => material.CreateUberMaterial()).ToArray());

            ZoneManagement.OnZoneChange += OnZoneChange;
            OnZoneChange(ZoneManagement.instance.zones);
        }

        public void OnZoneChange(ZoneData[] zones)
        {
            IEnumerable<GTZone> activeZones = zones.Where(zone => zone.active).Select(zone => zone.zone);
            OnZoneChange(activeZones.ToArray());
        }

        public void OnZoneChange(GTZone[] zones)
        {
            foreach (GTZone zone in zones)
            {
                if (locationFromZoneDict.TryGetValue(zone, out Location_Base location))
                {
                    bool useUberMaterials = zone == GTZone.ghostReactor;
                    if (uberMaterialsUsed != useUberMaterials)
                    {
                        uberMaterialsUsed = useUberMaterials;
                        renderers.ForEach(renderer => renderer.materials = uberMaterialsUsed ? uberMaterials[renderer] : materials[renderer]);
                    }
                    MoveStand(location.Position, location.EulerAngles);
                    return;
                }
            }

            Root.SetActive(false);
        }

        public void MoveStand(Transform transform) => MoveStand(transform.position, transform.eulerAngles);

        public void MoveStand(Vector3 position, Vector3 direction)
        {
            Root.transform.position = position;
            Root.transform.rotation = Quaternion.Euler(direction);
            Root.SetActive(true);
        }

        public void Update()
        {
            if (!PhotonNetwork.InRoom || PhotonNetworkController.Instance?.currentJoinTrigger == null)
                return;
        
            string zoneStr = PhotonNetworkController.Instance.currentJoinTrigger.networkZone;
        
            if (Enum.TryParse(zoneStr, out GTZone currentZone))
            {
                if (currentZone == GTZone.mines && !isInMines)
                {
                    Location_Mines minesLocation = new Location_Mines();
                    MoveStand(minesLocation.Position, minesLocation.EulerAngles);
                    isInMines = true;
                }
                else if (currentZone != GTZone.mines && isInMines)
                {
                    isInMines = false;
                }
            }
        }
#endif
    }
}
