@extends('layouts.app')
@section('title')
    Laravel 11 Image Intervention
@endsection
@section('content')
    @auth


        <div class="card mt-5">

            <div class="card-body">

                @if (count($errors) > 0)
                    <div class="alert alert-danger">
                        <strong>Whoops!</strong> There were some problems with your input.<br><br>
                        <ul>
                            @foreach ($errors->all() as $error)
                                <li>{{ $error }}</li>
                            @endforeach
                        </ul>
                    </div>
                @endif

                @session('success')
                    <div class="alert alert-success" role="alert">
                        {{ $value }}
                    </div>

                    <div class="row">
                        <div class="col-md-4">
                            <strong>Original Image:</strong>
                            <br />
                            <img src="/images/{{ Session::get('imageName') }}" width="300px" />
                        </div>
                        <div class="col-md-4">
                            <strong>Thumbnail Image:</strong>
                            <br />
                            <img src="/images/thumbnail/{{ Session::get('imageName') }}" />
                        </div>
                    </div>
                @endsession

                <form action="{{ route('image.store') }}" method="POST" enctype="multipart/form-data">
                    @csrf

                    <div class="mb-3">
                        <label class="form-label" for="inputImage">Image:</label>
                        <input type="file" name="image" id="inputImage"
                            class="form-control @error('image') is-invalid @enderror">

                        @error('image')
                            <span class="text-danger">{{ $message }}</span>
                        @enderror
                    </div>

                    <div class="mb-3">
                        <button type="submit" class="btn btn-success"><i class="fa fa-save"></i> Upload</button>
                    </div>

                </form>
            </div>
        </div>

    @endauth
@endsection
