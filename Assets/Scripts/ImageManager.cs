using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

//Force l'object sur lequel on met le script a avoir un ARTracked Image manager
[RequireComponent(typeof(ARTrackedImageManager))]
public class ImageManager : MonoBehaviour
{
    //Liste pour stocker les éléments que l'on fait spawn
    List<GameObject> spawnedList = new List<GameObject>();

    int spawnedCount = 0;

    //Serialized field : Permet d'exposer la variable objectToSpawn dans l'éditeur
    //objectToSpawn : Liste des préfabs que l'ont veut instancier
    [SerializeField]
    List<GameObject> objectsToSpawn = new List<GameObject>();
    
    //Référence au component ARTracked Image manager
    ARTrackedImageManager aRTrackedImageManager = null;

    //Fonction appelée à l'initialisation de l'objet
    private void Awake()
    {
        //On récupére la référence à l'ARTracked Image Manager
        aRTrackedImageManager = this.GetComponent<ARTrackedImageManager>();
    }

    //Fonction appelée à l'initialisation de l'objet, aprés le Awake
    private void OnEnable()
    {
        //On associe la fonction SwapOject (créé plus bas), a l'event déclenché par le ARTracked image manager lorsqu'il détecte une nouvelle image
        aRTrackedImageManager.trackedImagesChanged += SwapObject;
    }

    //Fonction appelée a la désactivation de l'objet
    private void OnDisable()
    {
        //On arrête d'écouter l'event (la fonction SwapObject ne sera plus appelée)
        aRTrackedImageManager.trackedImagesChanged -= SwapObject;
    }


    //Fonction appelée lorsque l'event trackedImagesChanged est déclenché par l'ARTracked image manager
    //Param : ARTrackedImagesChangedEventArgs trackedImages , arguement obligatoire , renvoyé par la fonction trackedImagesChanged
    public void SwapObject(ARTrackedImagesChangedEventArgs trackedImages)
    {
        //Tracked images contient des listes des images detectées selon leur statut, dans ce cas on parcours la liste des images ajoutées
        foreach (ARTrackedImage trackedImage in trackedImages.added)
        {
            //Si l'image ajoutée a le nom de la premiere image
            //on appelle la fonction SetNewMesh avec l'index correspondant a la liste des préfabs
            if(trackedImage.referenceImage.name == "unitylogowhiteonblack")
            {
                SetNewMesh(0);
            }
            //Sinon si l'image ajoutée a le nom de la seconde image
            //on appelle la fonction SetNewMesh avec l'index correspondant a la liste des préfabs
            else if (trackedImage.referenceImage.name == "one")
            {
                SetNewMesh(1);
            }
        }
    }

    //Fonction pour faire apparaite le mesh désiré
    public void SetNewMesh(int index)
    {
        //On cherche un objet de type SpawnedDefaut (Objet vide par défaut , avec comme seul composants un transform et le sript SpawnedDefault)

        SpawnedDefault[] objectsToReplace = GameObject.FindObjectsOfType<SpawnedDefault>();

        foreach(SpawnedDefault objectTotest in objectsToReplace)
        {
            if(objectTotest != null)
            {
                //Si la variable isSet de l'objet par défaut est a false
                if (!objectTotest.iSSet)
                {
                    //On instantie un objet de la liste via son index
                    GameObject newObject = Instantiate(objectsToSpawn[index], objectTotest.transform);
                    //On passe la variable de notre objet a true
                    objectTotest.iSSet = true;

                    //On attache notre nouvelle objet a l'objet vide par defaut
                    newObject.transform.parent = objectTotest.transform;
                    //On reset sa position locale (relative au nouveau parent) à 0,0,0
                    newObject.transform.localPosition = Vector3.zero;

                    spawnedList.Add(newObject);
                    //DEBUG on fait vibrer l'appareil pour voir si on passe bien dans la fonction
                    Handheld.Vibrate();

                }
            }
        }
    }
}
