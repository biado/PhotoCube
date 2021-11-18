import React, { useState, useEffect } from "react";
import "../../../css/GridBrowser.css";
import CubeObject from "../CubeBrowser/CubeObject";
import { BrowsingModes } from "../../RightDock/BrowsingModeChanger";
import Fetcher from "../CubeBrowser/Fetcher";
import { Image } from "../../../interfaces/types";
import { Filter } from "../../Filter";
import Modal from "./Modal";

/**
 * The GridBrowser allows the user to browse a collection of photos side by side in a grid to get an overview.
 * this.props.cubeObjects contains the cube object which photos are shown.
 * this.props.onBrowsingModeChanged is a callback funtion that tells parent component that the browsing mode has been changed.
 */
interface FuncProps {
  cubeObjects: CubeObject[];
  onBrowsingModeChanged: (browsingMode: BrowsingModes) => void;
  filters: Filter[];
  projectedFilters: Filter[];
  isProjected: boolean;
}

const GridBrowser: React.FC<FuncProps> = (props: FuncProps) => {
  const [images, setImages] = useState<Image[]>([]);

  const [modal, setModal] = useState<boolean>(false);

  const [imageTags, setImageTags] = useState<string[]>([]);

  const [imageId, setImageId] = useState<number>(0);

  const [imageFileUri, setImageFileUri] = useState<string>("");

  //const [imageDate, setImagedate] = useState<string>("");

  useEffect(() => {
    if (!props.isProjected) {
      fetchAllImages();
    } else {
      fetchWithProjection();
    }
    document.addEventListener("keydown", (e) => onKeydown(e));
    return () => {
      document.removeEventListener("keydown", (e) => onKeydown(e));
    };
  }, []);

/*   useEffect(() => {
    datetester()
  }, [imageTags]) */

  const fetchWithProjection = async () => {
    const allFilters = [...props.filters, ...props.projectedFilters];
    try {
      const response = await Fetcher.FetchAllImagesWithProjection(allFilters);
      setImages(response);
      console.log(response);
    } catch (error) {
      console.error(error);
    }
  };

  const fetchAllImages = async () => {
    try {
      const response = await Fetcher.FetchAllImages();
      setImages(response);
    } catch (error) {
      console.error(error);
    }
  };

  const onKeydown = (e: KeyboardEvent) => {
    console.log(e.key);
    if (e.key === "Escape") {
      props.onBrowsingModeChanged(BrowsingModes.Cube);
    }
  };

  const displayTagsInModal = (imageId: number, fileuri: string) => {
    setImageFileUri(fileuri);
    setImageId(imageId);
    fetchTags(imageId);
    toggleModal();
  };

  const toggleModal = () => {
    setModal(!modal);
  };

  const fetchTags = async (imageId: number) => {
    try {
      const response = await Fetcher.FetchTagsWithCubeObjectId(imageId);
      //console.log(response);
      setImageTags(response);
      
    } catch (error) {
      console.error(error);
    }
  };

/*   function isValidDate(d: any) {
    const goodDate = /^([1-9]|([012][0-9])|(3[01]))-([0]{0,1}[1-9]|1[012])-\d\d\d\d [012]{0,1}[0-9]:[0-9][0-9]:[0-9][0-9]$/
    return goodDate.test(d)
  } */

/*   function datetester() {
    imageTags.forEach((i) => {
      console.log(i)
      console.log(isValidDate(i))
      if (isValidDate(i)) {
        setImagedate(i)
      } 
    })
    console.log("THEDATE", imageDate)
  } */

  const opTimelineBrowser = async () => {
    console.log(imageId)
    try {
      const response = await Fetcher.FetchFromTimestamp(imageId);
      setImages(response);
    } catch (error) {}
  };

  return (
    <div className="grid-item">
      <div className="imageContainer">
        {images.length > 1000
          ? images.slice(0, 1000).map((image) => (
              <img
                onClick={() => displayTagsInModal(image.id, image.fileURI)}
                key={image.id}
                //title="foobar"
                className="image"
                src={process.env.REACT_APP_IMAGE_SERVER + image.fileURI}
              ></img>
            ))
          : images.map((image) => (
              <img
                onClick={() => displayTagsInModal(image.id, image.fileURI)}
                key={image.id}
                //title="foobar"
                className="image"
                src={process.env.REACT_APP_IMAGE_SERVER + image.fileURI}
              ></img>
            ))}
        <Modal
          show={modal}
          toggleModal={toggleModal}
          tags={imageTags}
          imageId={imageId}
          fileUri={imageFileUri}
          opTimelineBrowser={opTimelineBrowser}
        />
      </div>
    </div>
  );
};

export default GridBrowser;

/* export default class GridBrowser extends React.Component<{
    cubeObjects: CubeObject[],
    onBrowsingModeChanged: (browsingMode: BrowsingModes) => void
}>{

    state = {
        imagesInCell: []
    }
    render(){ */
/*       let images = this.props.cubeObjects.map((co, index) => <img 
            key={"image-"+index} 
            className="image" 
            src={process.env.REACT_APP_IMAGE_SERVER + co.fileURI}
            ></img>) */
/* 
            let images = this.state.imagesInCell.slice(0, 20).map(image => <img 
            //key={"image-"+index} 
            className="image" 
            src={process.env.REACT_APP_IMAGE_SERVER + image["fileURI"]}
            ></img>)

        return(
            <div className="grid-item">
                <div className="imageContainer">
                    {images}
                </div>
            </div>
        );
    } */

/* private async fetchAllImages() {
        await Fetcher.FetchAllImages().then((images:Object[]) => {
            this.setState({imagesInCell: images})
        });
    }
 */
/**
 * Component is to be shown. - Subscribe eventlisteners.
 */
/* componentDidMount(){
        document.addEventListener("keydown", (e) => this.onKeydown(e));
        this.fetchAllImages()
    } */

/**
 * Component is to be hidden. - Unsubscribe event listeners
 */
/*  componentWillUnmount(){
        document.removeEventListener("keydown", (e) => this.onKeydown(e));
    }
 */
/**
 * Handling Escape
 * @param e
 */
/* onKeydown(e: KeyboardEvent){
        //console.log(e.key);
        if(e.key === "Escape"){
            this.props.onBrowsingModeChanged(BrowsingModes.Cube);
        }
    }
} */
