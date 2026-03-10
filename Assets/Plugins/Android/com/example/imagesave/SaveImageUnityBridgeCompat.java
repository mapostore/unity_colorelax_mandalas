package com.example.imagesave;

import android.app.Activity;
import android.content.ContentResolver;
import android.content.ContentValues;
import android.content.Intent;
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
            Uri imageUri = saveBitmapToMediaStore(activity, bitmap, "ColorRelax_" + System.currentTimeMillis() + ".jpg");
            if (imageUri == null) {
                throw new RuntimeException("Failed to save image");
            }

            Toast.makeText(activity, "Image saved to gallery", Toast.LENGTH_SHORT).show();
        } catch (Exception e) {
            Log.e(TAG, "Save failed", e);
            Toast.makeText(activity, "Failed to save image", Toast.LENGTH_SHORT).show();
        } finally {
            bitmap.recycle();
        }
    }

    public static void ShareImage(byte[] data, String subject, String title, String message) {
        Activity activity = UnityPlayer.currentActivity;
        if (activity == null) {
            return;
        }
        if (data == null || data.length == 0) {
            Toast.makeText(activity, "Nothing to share", Toast.LENGTH_SHORT).show();
            return;
        }

        Bitmap bitmap = BitmapFactory.decodeByteArray(data, 0, data.length);
        if (bitmap == null) {
            Toast.makeText(activity, "Invalid image data", Toast.LENGTH_SHORT).show();
            return;
        }

        try {
            Uri imageUri = saveBitmapToMediaStore(activity, bitmap, "ColorRelax_share_" + System.currentTimeMillis() + ".jpg");
            if (imageUri == null) {
                throw new RuntimeException("Failed to prepare image");
            }

            Intent sendIntent = new Intent(Intent.ACTION_SEND);
            sendIntent.setType("image/*");
            sendIntent.putExtra(Intent.EXTRA_SUBJECT, subject);
            sendIntent.putExtra(Intent.EXTRA_TITLE, title);
            sendIntent.putExtra(Intent.EXTRA_TEXT, message);
            sendIntent.putExtra(Intent.EXTRA_STREAM, imageUri);
            sendIntent.addFlags(Intent.FLAG_GRANT_READ_URI_PERMISSION);

            Intent chooser = Intent.createChooser(sendIntent, "Share image");
            activity.startActivity(chooser);
        } catch (Exception e) {
            Log.e(TAG, "Share failed", e);
            Toast.makeText(activity, "Failed to share image", Toast.LENGTH_SHORT).show();
        } finally {
            bitmap.recycle();
        }
    }

    private static Uri saveBitmapToMediaStore(Activity activity, Bitmap bitmap, String fileName) throws Exception {
        ContentResolver resolver = activity.getContentResolver();
        ContentValues values = new ContentValues();
        values.put(MediaStore.Images.Media.DISPLAY_NAME, fileName);
        values.put(MediaStore.Images.Media.MIME_TYPE, "image/jpeg");

        if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.Q) {
            values.put(MediaStore.Images.Media.RELATIVE_PATH, Environment.DIRECTORY_PICTURES + "/ColorRelax");
            values.put(MediaStore.Images.Media.IS_PENDING, 1);
        } else {
            File picturesDir = Environment.getExternalStoragePublicDirectory(Environment.DIRECTORY_PICTURES);
            File outDir = new File(picturesDir, "ColorRelax");
            if (!outDir.exists() && !outDir.mkdirs()) {
                throw new RuntimeException("Failed to create output directory");
            }
            File imageFile = new File(outDir, fileName);
            values.put(MediaStore.Images.Media.DATA, imageFile.getAbsolutePath());
        }

        Uri imageUri = resolver.insert(MediaStore.Images.Media.EXTERNAL_CONTENT_URI, values);
        if (imageUri == null) {
            throw new RuntimeException("Failed to create MediaStore record");
        }

        try (OutputStream out = resolver.openOutputStream(imageUri)) {
            if (out == null || !bitmap.compress(Bitmap.CompressFormat.JPEG, 90, out)) {
                throw new RuntimeException("Failed to write image");
            }
        }

        if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.Q) {
            ContentValues pendingOff = new ContentValues();
            pendingOff.put(MediaStore.Images.Media.IS_PENDING, 0);
            resolver.update(imageUri, pendingOff, null, null);
        }

        return imageUri;
    }
}
