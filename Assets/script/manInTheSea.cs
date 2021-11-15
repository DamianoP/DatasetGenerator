using System.Collections;
using System.IO;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

public class manInTheSea : MonoBehaviour{
    public GameObject objectsContainer;
    public GameObject seaObj1;
    public GameObject seaObj2;
    public GameObject seaObj3;
    public GameObject seaObj4;
    public GameObject man1;
    public GameObject man2;
    public GameObject man3;
    public GameObject man4;
    public GameObject man5;
    public GameObject man6;
    public GameObject man7;
    public GameObject man8;
    public GameObject man9;
    public GameObject man10;
    public Light light1;
    public int numberOfImages = 100;
    public bool cameraRotation = true;
    public int minRotation = -45;
    public int maxRotation = +45;
    
    public bool lightRotation = true;

    public bool capturePic = true;
    public bool scalePicture = true;
    public int picResolution = 299;

    // PRIVATE VALUES
    public string outputPath = "Z:/screen/";
    private int imgCounter = 0;

    void Start(){
        datasetGen();
    }

    public void datasetGen(){
        StartCoroutine(datasetGenEncaps());
    }

    IEnumerator datasetGenEncaps(){
        string className = "false";
        bool falseObj = true;
        disableAllObjects();
        /*
         =========||=========||========||========
         ONLY SEA || SEA+OBJ || MAN IN THE SEA
            25%   ||  25%    || 50%
            50% false        || 50% true
         
         */
        for (int i = 0; i < numberOfImages; i++){
            moveWordSpace();
            if (i > (2*numberOfImages/4)){
                if (falseObj){
                    disableSeaObj();
                    falseObj = false;
                    className = "true";
                }
                spawnMan();
            }
            else if (i > (1*numberOfImages/4)){  
                spawnObjects();
            }
            if (lightRotation) randomLight(light1);
            if (capturePic){
                yield return new WaitForSeconds(0.0001f);
                yield return new WaitForEndOfFrame();
                string outImage = outputPath + className + "_" + imgCounter + ".png";
                Texture2D screenImage = new Texture2D(Screen.width, Screen.height);
                //Get Image from screen
                screenImage.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
                screenImage.Apply();
                //Convert to png
                byte[] imageBytes;
                if (scalePicture){
                    Texture2D newScreenshot = ScaleScreenshot(screenImage, picResolution, picResolution);
                    imageBytes = newScreenshot.EncodeToPNG();
                }
                else{
                    imageBytes = screenImage.EncodeToPNG();
                }
                //Save image to file
                System.IO.File.WriteAllBytes(outImage, imageBytes);
                imgCounter++;
                Debug.Log("Saving pic " + outImage);
            }
        }
        EditorApplication.ExecuteMenuItem("Edit/Play");

        Debug.Log("Execution completed");
    }

    private void moveWordSpace(){
        rotate(objectsContainer, 1, Random.Range(minRotation, maxRotation), 1);
        int x = Random.Range(380, 422);
        float y = -0.5373687f;
        int z = Random.Range(40, 26);
        objectsContainer.transform.position = new Vector3(x,y,z);
    }
    private void spawnObjects(){
        launchCoinAndSpanwObj(seaObj1);
        launchCoinAndSpanwObj(seaObj2);
        launchCoinAndSpanwObj(seaObj3);
        launchCoinAndSpanwObj(seaObj4);
    }

    private void launchCoinAndSpanwObj(GameObject obj){
        if(Random.Range(0, 2)==0) obj.SetActive(false);
        else obj.SetActive(true);
    }
    private void randomLight(Light light){
        int x = Random.Range(13, 100);
        int y = Random.Range(0, 360);
        int z = Random.Range(0, 1);
        GameObject lightObj = light.gameObject;
        rotate(lightObj, x, y, z);
        light.intensity=Random.Range(0.8f, 1.21f);
    }

    IEnumerator takePicture(string className){
        yield return new WaitForSeconds(0.0001f);
        yield return new WaitForEndOfFrame();
        string path = outputPath + className + "_" + imgCounter + ".png";
        Texture2D screenImage = new Texture2D(Screen.width, Screen.height);
        //Get Image from screen
        screenImage.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        screenImage.Apply();
        //Convert to png
        byte[] imageBytes;
        if (scalePicture){
            Texture2D newScreenshot = ScaleScreenshot(screenImage, picResolution, picResolution);
            imageBytes = newScreenshot.EncodeToPNG();
        }
        else{
            imageBytes = screenImage.EncodeToPNG();
        }

        //Save image to file
        System.IO.File.WriteAllBytes(path, imageBytes);
        imgCounter++;
        Debug.Log("Saving pic " + path);
    }

    private Texture2D ScaleScreenshot(Texture2D source, int targetWidth, int targetHeight){
        Texture2D result = new Texture2D(targetWidth, targetHeight, source.format, true);
        Color[] rpixels = result.GetPixels(0);
        float incX = ((float) 1 / source.width) * ((float) source.width / targetWidth);
        float incY = ((float) 1 / source.height) * ((float) source.height / targetHeight);
        for (int px = 0; px < rpixels.Length; px++){
            rpixels[px] = source.GetPixelBilinear(incX * ((float) px % targetWidth),
                incY * ((float) Mathf.Floor(px / targetWidth)));
        }

        result.SetPixels(rpixels, 0);
        result.Apply();
        return result;
    }

    private void randomRotation(GameObject obj){
        int x = Random.Range(minRotation, maxRotation);
        int y = Random.Range(minRotation, maxRotation);
        int z = Random.Range(minRotation, maxRotation);
        rotate(obj, x, y, z);
    }

    private void rotate(GameObject obj, float x, float y, float z){
        obj.transform.eulerAngles = new Vector3(
            x,
            y,
            z
        );
    }

    private static Texture2D loadPNG(string filePath){
        Texture2D tex = null;
        byte[] fileData;

        if (File.Exists(filePath)){
            fileData = File.ReadAllBytes(filePath);
            tex = new Texture2D(2, 2);
            tex.LoadImage(fileData); //..this will auto-resize the texture dimensions.
        }

        return tex;
    }

    private void disableSeaObj(){
        seaObj1.SetActive(false);
        seaObj2.SetActive(false);
        seaObj3.SetActive(false);
        seaObj4.SetActive(false);
    }
    private void disableMan(){
        man1.SetActive(false);
        man2.SetActive(false);
        man3.SetActive(false);
        man4.SetActive(false);
        man5.SetActive(false);
        man6.SetActive(false);
        man7.SetActive(false);
        man8.SetActive(false);
        man9.SetActive(false);
        man10.SetActive(false);
    }

    private void disableAllObjects(){
        disableSeaObj();
        disableMan();
    }
    
    private void spawnMan(){
        launchCoinAndSpanwObj(man1);
        launchCoinAndSpanwObj(man2);
        launchCoinAndSpanwObj(man3);
        launchCoinAndSpanwObj(man4);
        launchCoinAndSpanwObj(man5);
        launchCoinAndSpanwObj(man6);
        launchCoinAndSpanwObj(man7);
        launchCoinAndSpanwObj(man8);
        launchCoinAndSpanwObj(man9);
        launchCoinAndSpanwObj(man10);
    }
}