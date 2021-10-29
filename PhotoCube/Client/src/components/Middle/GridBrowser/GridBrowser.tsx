import React from 'react';
import '../../../css/GridBrowser.css';
import CubeObject from '../CubeBrowser/CubeObject';
import { BrowsingModes } from '../../RightDock/BrowsingModeChanger';
import Fetcher from '../CubeBrowser/Fetcher';

/**
 * The GridBrowser allows the user to browse a collection of photos side by side in a grid to get an overview.
 * this.props.cubeObjects contains the cube object which photos are shown.
 * this.props.onBrowsingModeChanged is a callback funtion that tells parent component that the browsing mode has been changed.
 */
export default class GridBrowser extends React.Component<{
    cubeObjects: CubeObject[],
    onBrowsingModeChanged: (browsingMode: BrowsingModes) => void
}>{

    state = {
        imagesInCell: []
    }
    render(){
  /*       let images = this.props.cubeObjects.map((co, index) => <img 
            key={"image-"+index} 
            className="image" 
            src={process.env.REACT_APP_IMAGE_SERVER + co.fileURI}
            ></img>) */

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
    }

    private async fetchAllImages() {
        await Fetcher.FetchAllImages().then((images:Object[]) => {
            this.setState({imagesInCell: images})
        });
    }

    /**
     * Component is to be shown. - Subscribe eventlisteners.
     */
    componentDidMount(){
        document.addEventListener("keydown", (e) => this.onKeydown(e));
        this.fetchAllImages()
    }

    /**
     * Component is to be hidden. - Unsubscribe event listeners
     */
    componentWillUnmount(){
        document.removeEventListener("keydown", (e) => this.onKeydown(e));
    }

    /**
     * Handling Escape
     * @param e 
     */
    onKeydown(e: KeyboardEvent){
        //console.log(e.key);
        if(e.key === "Escape"){
            this.props.onBrowsingModeChanged(BrowsingModes.Cube);
        }
    }
}