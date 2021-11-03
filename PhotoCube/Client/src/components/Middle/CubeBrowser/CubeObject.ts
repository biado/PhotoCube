/**
 * PhotoCube Client repressentation of an Object in the M^3 datamodel.
 * Is very similar to PublicCubeObject.cs in PhotoCube Server implementation.
 */
export default interface CubeObject{
    id: number,
    fileURI: string,
}