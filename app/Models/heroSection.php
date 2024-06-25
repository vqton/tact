<?php

namespace App\Models;

use Illuminate\Database\Eloquent\Factories\HasFactory;
use Illuminate\Database\Eloquent\Model;

class heroSection extends Model
{
    use HasFactory;
    protected $table = 'ld_hero';

    public const CREATED_AT = 'creation_date';
    public const UPDATED_AT = 'updated_date';
}
