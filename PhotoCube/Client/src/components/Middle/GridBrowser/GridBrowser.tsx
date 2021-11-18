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

  const [imageTags, setImageTags] = useState<string[]>(["foo", "bar"]);

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

  const submitImage = async (fileUri: string) => {
    try {
      Fetcher.SubmitImage(fileUri).then((r) => {
        console.log(r);
      });
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

  const displayTags = () => {
    toggleModal()
  }

  const toggleModal = () => {
    setModal(!modal);
  };

  const fetchTags = async (imageId: number) => {
    try {
      const response = await Fetcher.FetchTagsWithCubeObjectId(imageId);
      setImageTags(response)
    } catch (error) {
      console.error(error)
    }
  };

  return (
    <div className="grid-item">
      <div className="imageContainer">
        {images.length > 100
          ? images.slice(0, 100).map((image) => (
                <img
                  onDoubleClick={() => submitImage(image.fileURI)}
                  onMouseOver={() => fetchTags(image.id)}
                  onClick={() => displayTags()}
                  key={image.id}
                  className="image"
                  src={process.env.REACT_APP_IMAGE_SERVER + image.fileURI}
                ></img>
            ))
          : images.map((image) => (
              <img
                onDoubleClick={() => submitImage(image.fileURI)}
                onMouseOver={() => fetchTags(image.id)}
                onClick={() => displayTags()}
                key={image.id}
                className="image"
                src={process.env.REACT_APP_IMAGE_SERVER + image.fileURI}
              ></img>
            ))}
        <Modal show={modal} toggleModal={toggleModal} tags={imageTags}/>
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
