using System.Collections;
using System.IO;
using UnityEngine;
using Random = UnityEngine.Random;
using UnityEditor;

public class genDataset : MonoBehaviour{
    public Material material;
    public GameObject obj;
    public Light light1;
    public int numberOfImages = 100;
    public bool objRotation = true;
    public int minRotation = -45;
    public int maxRotation = +45;
    public bool lightRotation = true;
    public bool stretchObj = true;

    public bool singleTexture = false;
    public bool capturePic = true;
    public bool scalePicture = true;
    public float textureScalingFactor = 1.8f;
    public int picResolution = 299;

    // PRIVATE VALUES
    public string inputPath = "Z:/input/";
    public string[] classes = new string[]{"type1", "type2"};
    public string outputPath = "Z:/screen/";
    private int imgCounter = 0;

    void Start(){
        datasetGen();
    }

    public void datasetGen(){
        if (singleTexture){
            StartCoroutine(onlyDataAugmentation());
        }
        else{
            StartCoroutine(datasetGenEncaps());
        }
    }

    IEnumerator datasetGenEncaps(){
        foreach (var className in classes){
            imgCounter = 0;
            string path = inputPath + className;
            var info = new DirectoryInfo(path);
            var filesInfo = info.GetFiles();
            foreach (var file in filesInfo){
                string filepath = file.ToString();
                Debug.Log("File founded: " + filepath);
                Texture2D texture = loadPNG(filepath);
                material.mainTexture = texture;
                adjustTextureAndObjectSize(texture);
                for (int i = 0; i < numberOfImages; i++){
                    dataAugmentationObject();
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
            }
        }
        Debug.Log("Esecuzione terminata");
        EditorApplication.ExecuteMenuItem("Edit/Play");
    }

    private void adjustTextureAndObjectSize(Texture2D texture){
        Debug.Log("Size is " + texture.width + " by " + texture.height);
        float x = texture.width;
        float y = texture.height;
        if (x > y){
            y = textureScalingFactor * y / x;
            x = textureScalingFactor;
        }
        else{
            x = textureScalingFactor * x / y;
            y = textureScalingFactor;
        }
        obj.transform.localScale = new Vector3(x, y, 0.01f);
    }

    private void dataAugmentationObject(){
        if (objRotation){
            randomRotation(obj);
        }
        if (lightRotation){
            randomLight(light1);
        }
        if (stretchObj){
            stretchObject(obj);
        }
    }

    private void stretchObject(GameObject obj){
        float x = Random.Range(75, 110);
        float y = Random.Range(75, 110);
        float z = Random.Range(75, 110);
        obj.transform.localScale = new Vector3(
            x / 100,
            y / 100,
            z / 100
        );
    }

    private void randomLight(Light light){
        int x = Random.Range(13, 60);
        int y = Random.Range(-15, 15);
        int z = Random.Range(1, 2);
        GameObject lightObj = light.gameObject;
        rotate(lightObj, x, y, z);
        //light.intensity=Random.Range(minLight, maxLight);
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
    
    
    IEnumerator onlyDataAugmentation(){
        for (int i = 0; i < numberOfImages; i++){
            dataAugmentationObject();
            if (capturePic){
                yield return new WaitForSeconds(0.0001f);
                yield return new WaitForEndOfFrame();
                string outImage = outputPath + imgCounter + ".png";
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
    }
}