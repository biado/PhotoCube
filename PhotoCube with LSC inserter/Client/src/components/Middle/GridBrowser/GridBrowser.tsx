import React, { Component, SyntheticEvent } from 'react';
import '../../../css/GridBrowser.css';
import CubeObjectFileURI from '../ThreeBrowser/CubeObjectFileURI';
import Fetcher from '../ThreeBrowser/Fetcher';
import { BrowsingModes } from '../../RightDock/BrowsingModeChanger';

/**
 * The GridBrowser allows the user to browse a collection of photos side by side in a grid to get an overview.
 * this.props.cubeObjectFileURIs contains the cube object which photos are shown.
 * this.props.onBrowsingModeChanged is a callback funtion that tells parent component that the browsing mode has been changed.
 */
export default class GridBrowser extends React.Component<{
    cubeObjectFileURIs: CubeObjectFileURI[],
    onBrowsingModeChanged: (browsingMode: BrowsingModes) => void
}>{
    render(){
        let images = this.props.cubeObjectFileURIs.map((co, index) => <img 
            key={"image-"+index} 
            className="image" 
            src={process.env.REACT_APP_IMAGE_SERVER + co.FileURI}
            ></img>)

        return(
            <div className="grid-item">
                <div className="imageContainer">
                    {images}
                </div>
            </div>
        );
    }

    /**
     * Component is to be shown. - Subscribe eventlisteners.
     */
    componentDidMount(){
        document.addEventListener("keydown", (e) => this.onKeydown(e));
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
        if(e.key == "Escape"){
            this.props.onBrowsingModeChanged(BrowsingModes.Cube);
        }
    }
}