using System.Collections.Generic;
using System.IO;
using System.Drawing;                 // System.Drawing.Bitmap
using System.Drawing.Imaging;         // ImageFormat
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DynamicToggle : MonoBehaviour
{
    [SerializeField] private GameObject togglePrefab;
    [SerializeField] private Transform container;

    private void Start()
    {
        LoadProcessesUI();
    }

    public void LoadProcessesUI()
    {
        foreach (Transform child in container)
            Destroy(child.gameObject);

        List<UserInstalledAppsReader.AppInfo> processes = UserInstalledAppsReader.GetUserInstalledApplications();

        foreach (var app in processes)
        {
            GameObject newToggleObj = Instantiate(togglePrefab, container);

            var legacyText = newToggleObj.GetComponentInChildren<Text>();
            if (legacyText != null) legacyText.text = app.DisplayName;

            var tmpText = newToggleObj.GetComponentInChildren<TMP_Text>();
            if (tmpText != null) tmpText.text = app.DisplayName;
            
           /* // Иконка — если в InstalledApp хранится System.Drawing.Bitmap или byte[] PNG
            var iconImage = newToggleObj.transform.Find("Icon")?.GetComponent<UnityEngine.UI.Image>();
            if (iconImage != null && app.Icon != null)
            {
                // Преобразовать System.Drawing.Bitmap в Unity Texture2D
                Texture2D tex = Texture2DFromBitmap(app.Icon);
                if (tex != null)
                {
                    iconImage.sprite = Sprite.Create(
                        tex,
                        new Rect(0, 0, tex.width, tex.height),
                        new Vector2(0.5f, 0.5f));
                }
            }
           */
        }
    }

   /* // Метод для вывода картинки в Unity из System.Drawing.Bitmap

    private static Texture2D Texture2DFromBitmap(Bitmap bmp)
    {
        if (bmp == null) return null;

        using (var ms = new MemoryStream())
        {
            // Сохранить bitmap как PNG в поток памяти
            bmp.Save(ms, ImageFormat.Png);
            byte[] pngData = ms.ToArray();

            // Создать Texture2D и загрузить PNG-данные
            Texture2D tex = new Texture2D(2, 2, TextureFormat.RGBA32, false);
            if (tex.LoadImage(pngData))
            {
                return tex;
            }
            UnityEngine.Object.Destroy(tex);
            return null;
        }
    }
   */
}