package com.example.imagesave;

import android.app.Activity;
import android.content.ContentResolver;
import android.content.ContentValues;
import android.graphics.Bitmap;
import android.graphics.BitmapFactory;
import android.net.Uri;
import android.os.Build;
import android.os.Environment;
import android.provider.MediaStore;
import android.util.Log;
import android.widget.Toast;

import com.unity3d.player.UnityPlayer;

import java.io.File;
import java.io.FileOutputStream;
import java.io.OutputStream;

public class SaveImageUnityBridgeCompat {
    private static final String TAG = "SaveImageCompat";

    public static void CallSaveImage(byte[] data) {
        Activity activity = UnityPlayer.currentActivity;
        if (activity == null) {
            return;
        }

        if (data == null || data.length == 0) {
            Toast.makeText(activity, "Nothing to save", Toast.LENGTH_SHORT).show();
            return;
        }

        Bitmap bitmap = BitmapFactory.decodeByteArray(data, 0, data.length);
        if (bitmap == null) {
            Toast.makeText(activity, "Invalid image data", Toast.LENGTH_SHORT).show();
            return;
        }

        try {
            String fileName = "ColorRelax_" + System.currentTimeMillis() + ".jpg";
            ContentResolver resolver = activity.getContentResolver();
            Uri imageUri;

            if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.Q) {
                ContentValues values = new ContentValues();
                values.put(MediaStore.Images.Media.DISPLAY_NAME, fileName);
                values.put(MediaStore.Images.Media.MIME_TYPE, "image/jpeg");
                values.put(MediaStore.Images.Media.RELATIVE_PATH, Environment.DIRECTORY_PICTURES + "/ColorRelax");
                values.put(MediaStore.Images.Media.IS_PENDING, 1);

                imageUri = resolver.insert(MediaStore.Images.Media.EXTERNAL_CONTENT_URI, values);
                if (imageUri == null) {
                    throw new RuntimeException("Failed to create MediaStore record");
                }

                try (OutputStream out = resolver.openOutputStream(imageUri)) {
                    if (out == null || !bitmap.compress(Bitmap.CompressFormat.JPEG, 90, out)) {
                        throw new RuntimeException("Failed to write image");
                    }
                }

                values.clear();
                values.put(MediaStore.Images.Media.IS_PENDING, 0);
                resolver.update(imageUri, values, null, null);
            } else {
                File picturesDir = Environment.getExternalStoragePublicDirectory(Environment.DIRECTORY_PICTURES);
                File outDir = new File(picturesDir, "ColorRelax");
                if (!outDir.exists() && !outDir.mkdirs()) {
                    throw new RuntimeException("Failed to create output directory");
                }

                File imageFile = new File(outDir, fileName);
                try (FileOutputStream out = new FileOutputStream(imageFile)) {
                    if (!bitmap.compress(Bitmap.CompressFormat.JPEG, 90, out)) {
                        throw new RuntimeException("Failed to write image");
                    }
                }

                MediaStore.Images.Media.insertImage(
                        resolver,
                        imageFile.getAbsolutePath(),
                        imageFile.getName(),
                        imageFile.getName()
                );
            }

            Toast.makeText(activity, "Image saved to gallery", Toast.LENGTH_SHORT).show();
        } catch (Exception e) {
            Log.e(TAG, "Save failed", e);
            Toast.makeText(activity, "Failed to save image", Toast.LENGTH_SHORT).show();
        } finally {
            bitmap.recycle();
        }
    }
}
