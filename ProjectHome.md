[IKON](IKON.md) (Ivan Kravarščan's object notation) is JSON like object notation based on [IKADN](IKADN.md) and an example how can IKADN be used create object notation that is human readable and supports various types of software data. Example of serialized IKON data:

```
=3.14159 @pi

{ Model
    name "A-10"
    x =2.1
    y =-3.4
    angle #pi
    materials [
        =0
        =1
        =2
    ]
    material0 { Material
        alpha =1
    }
} 
```

Available on [NuGet](https://www.nuget.org/)