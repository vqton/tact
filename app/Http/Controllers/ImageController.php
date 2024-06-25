<?php

namespace App\Http\Controllers;

use Illuminate\Http\Request;
use Illuminate\Support\Facades\Storage;

use Intervention\Image\Laravel\Facades\Image;

class ImageController extends Controller
{
    //

    /**
     * Display a listing of the resource.
     *
     * @return \Illuminate\Http\Response
     */
    public function index()
    {
        return view('uploadimage');
    }

    /**
     * Display a listing of the resource.
     *
     * @return \Illuminate\Http\Response
     */
    /*public function store(Request $request): RedirectResponse
    {
        $this->validate($request, [
            'image' => 'required|image|mimes:jpeg,png,jpg,gif,svg|max:2048',
        ]);

        $image = Image::read($request->file('image'));

        // Main Image Upload on Folder Code
        $imageName = time().'-'.$request->file('image')->getClientOriginalName();

        $destinationPath = public_path('images/');
        $image->save($destinationPath.$imageName);

        // Generate Thumbnail Image Upload on Folder Code
        $destinationPathThumbnail = public_path('images/thumbnail/');
        $image->resize(100, 100);
        $image->save($destinationPathThumbnail.$imageName);



        return redirect()->back()
            ->with('success', 'Image Upload successful')
            ->with('imageName', $imageName);
    }*/
    public function store(Request $request)
    {
        $this->validate($request, [
            'image' => 'required|image|mimes:jpeg,png,jpg,gif,svg|max:2048',
        ]);

        // Generate unique filename
        $imageName = time() . '-' . $request->file('image')->getClientOriginalName();

        // Store image in public disk using Storage facade
        $image = Image::read($request->file('image')); // Read image using Intervention Image

        // $image->save(Storage::disk('public')->path('images/' . $imageName)); // Save to public/images
        $destinationPath = public_path('images/');
        $image->save($destinationPath.$imageName);

        // Optional: Generate thumbnail (adjust dimensions as needed)
        $destinationPathThumbnail = public_path('images/thumbnail/');
        $image->resize(100, 100);
        $image->save($destinationPathThumbnail.$imageName);

        // Placeholder for your image upload logic (adjust as needed)
        // For example, storing information about the uploaded image in a database:
        // $upload = new Image; // Assuming you have a model named Image
        // $upload->filename = $imageName;
        // $upload->save();

        return redirect()->back()
            ->with('success', 'Image Upload successful')
            ->with('imageName', $imageName);
    }
}
