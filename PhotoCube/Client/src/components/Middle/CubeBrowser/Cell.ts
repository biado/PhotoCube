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

    imageIndex: number = 0;

    constructor(
        scene:THREE.Scene, 
        textureLoader:THREE.TextureLoader, 
        addCubeCallback: (imageUrl: string, aPosition: Position) => THREE.Mesh, aPosition:Position, 
        cubeObjectData: CubeObject[],
        countOfImages: number
        ){

        this.count = countOfImages;
        
        this.scene = scene;
        this.textureLoader = textureLoader;
        
        this.x = aPosition.x;
        this.y = aPosition.y;
        this.z = aPosition.z;
        this.CubeObjects = cubeObjectData;
        if(cubeObjectData.length > 0){
            let spotifyURI: string = this.CubeObjects[0].fileURI
            let imgsrc: string = this.CubeObjects[0].thumbnailURI
            //let imgsrc: string = "https://i.scdn.co/image/ab67616d00004851"+this.CubeObjects[0].thumbnailURI
            if (imgsrc.length==24) { 
                imgsrc = "https://i.scdn.co/image/ab67616d00004851"+imgsrc // using 64x64 sp_thumbnail
            } else if (imgsrc.length>24) {
                imgsrc = "https://i.scdn.co/image/"+imgsrc // odd path
            } else if (imgsrc.length<24) {
                imgsrc = "file:///home/ek/Documents/Thesis/PhotoCube/PhotoCube/Client/src/images/colors/"+this.CubeObjects[0].color // missing thumbnail - using color
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
