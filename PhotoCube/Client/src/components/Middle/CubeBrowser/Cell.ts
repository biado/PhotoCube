import CubeObject from './CubeObject';
import * as THREE from 'three';
import Position from './Position';

/**
 * Interface representing a primitive cell object. Used to create cells classes from.
 */
export default interface ICell{
    //Coordinates:
    x: number;
    y: number;
    z: number;

    //count of images
    count: number;

    //Data nessesary for cube to get own data.
    cubeObjects: CubeObject[];
}

/**
 * Repressents a Cell in the hypercube in the CubeBrowser.
 */
export default class Cell{
    //Reference to Scene.
    scene: THREE.Scene;
    textureLoader: THREE.TextureLoader;
    
    //Coordinates in hyperCube:
    x: number;
    y: number;
    z: number;

    //the count property references the number of images in the cell
    count: number;

    //Data nessesary for cube to get own data.
    CubeObjects: CubeObject[];
    threeObject: THREE.Mesh | null;

    useColor:boolean; //use color thumbnail for big browsing states

    imageIndex: number = 0;

    constructor(
        scene:THREE.Scene, 
        textureLoader:THREE.TextureLoader, 
        addCubeCallback: (imageUrl: string, aPosition: Position) => THREE.Mesh, 
        aPosition:Position, 
        cubeObjectData: CubeObject[],
        countOfImages: number,
        useColor: boolean
        ){

        this.count = countOfImages;
        
        this.scene = scene;
        this.textureLoader = textureLoader;
        
        this.x = aPosition.x;
        this.y = aPosition.y;
        this.z = aPosition.z;
        this.CubeObjects = cubeObjectData;
        this.useColor = useColor;
        if(cubeObjectData.length > 0){
            //let spotifyURI: string = this.CubeObjects[0].fileURI
            let imgsrc :string = "";
            if(useColor){
                // coloring based on filecount (power of 2)
                if (this.count == 1){
                    imgsrc = "/shades/0000b8.png"
                }
                if (this.count > 1 && this.count <= 2){
                    imgsrc = "/shades/1515be.png"
                }
                if (this.count > 2 && this.count <= 4){
                    imgsrc = "/shades/2b2bc4.png"
                }
                if (this.count > 4 && this.count <= 8){
                    imgsrc = "/shades/4040ca.png"
                }
                if (this.count > 8 && this.count <= 16){
                    imgsrc = "/shades/5555d0.png"
                }
                if (this.count > 16 && this.count <= 32){
                    imgsrc = "/shades/6a6ad6.png"
                }
                if (this.count > 32 && this.count <= 64){
                    imgsrc = "/shades/8080db.png"
                }
                if (this.count > 64 && this.count <= 128){
                    imgsrc = "/shades/9595e1.png"
                }
                if (this.count > 128 && this.count <= 256){
                    imgsrc = "/shades/aaaae7.png"
                }
                if (this.count > 256 && this.count <= 512){
                    imgsrc = "/shades/bfbfed.png"
                }
                if (this.count > 512 && this.count <= 1024){
                    imgsrc = "/shades/d4d4f3.png"
                }
                if (this.count > 1024 && this.count <= 2048){
                    imgsrc = "/shades/eaeaf9.png"
                }
                if (this.count > 2048 ){
                    imgsrc = "/shades/ffffff.png"
                }
            }else{
                imgsrc = this.CubeObjects[0].thumbnailURI
                if (imgsrc.length==24) {
                    imgsrc = "https://i.scdn.co/image/ab67616d00004851"+imgsrc // using 64x64 sp_thumbnail
                } else if (imgsrc.length>24) {
                    imgsrc = "https://i.scdn.co/image/"+imgsrc // odd path
                } else if (imgsrc.length<24) {
                    imgsrc = "/colors/"+"download.jpg" //*missing thumbnail - using replacement cat
                }
            }
            this.threeObject = addCubeCallback(imgsrc, {x: this.x, y: this.y, z:this.z});
            this.threeObject.userData = { x: this.x, y: this.y, z:this.z, size: this.count, cubeObjects: this.CubeObjects };
            //this.ToggleSwitchingImagesEveryXms(10000);
        } 
        else{
            this.threeObject = null;
        }
    }

    RemoveFromScene(){
        if(this.threeObject != null) this.scene.remove(this.threeObject);
    }

    //Not in use:
    /* ToggleSwitchingImagesEveryXms(miliseconds: number){
        setInterval(() => {
            this.imageIndex++;
            let nextImage = this.imageIndex % this.CubeObjects.length;
            this.threeObject!.material = new THREE.MeshBasicMaterial({
                map : this.textureLoader.load(Fetcher.baseUrl + "/thumbnail/" + 
                    this.CubeObjects[nextImage].ThumbnailId)
            });
            //Do something each 'miliseconds' seconds  
        }, miliseconds);
    } */
   
}
