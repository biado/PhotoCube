import React, { Component, SyntheticEvent } from 'react';
import '../../../css/GridBrowser.css';
import CubeObject from '../ThreeBrowser/CubeObject';
import Fetcher from '../ThreeBrowser/Fetcher';
import { BrowsingModes } from '../../RightDock/BrowsingModeChanger';
import PhotoCubeClient from '../../PhotoCubeClient';

/**
 * The GridBrowser allows the user to browse a collection of photos side by side in a grid to get an overview.
 * this.props.cubeObjects contains the cube object which photos are shown.
 * this.props.onBrowsingModeChanged is a callback funtion that tells parent component that the browsing mode has been changed.
 */
export default class GridBrowser extends React.Component<{
    cubeObjects: CubeObject[],
    onBrowsingModeChanged: (browsingMode: BrowsingModes) => void,
    onLoadMore: () => void
}>{
    state = {
        cubeObjects: []
      }

    render() {
        let images = this.props.cubeObjects.map((co, index) => <img
            key={"image-" + index}
            className="image"
            src={process.env.REACT_APP_IMAGE_SERVER + co.FileURI}
        ></img>)

        return (
            <div className="grid-item">
                <div className="imageContainer">
                    {images}
                </div>
                <div>
                    <button onClick={(e) => this.props.onLoadMore()}>Load more</button>
                </div>
            </div>
        );
    }

    /**
     * Component is to be shown. - Subscribe eventlisteners.
     */
    componentDidMount() {
        document.addEventListener("keydown", (e) => this.onKeydown(e));
    }

    componentDidUpdate() {
        console.log(this.state.cubeObjects);
        console.log(this.props.cubeObjects);
        if (this.state.cubeObjects !== this.props.cubeObjects) {
            this.setState({cubeObjects: this.props.cubeObjects}, () => {console.log("CubeObject[] in Grid updated.")});
        }
    }

    /**
     * Component is to be hidden. - Unsubscribe event listeners
     */
    componentWillUnmount() {
        document.removeEventListener("keydown", (e) => this.onKeydown(e));
    }

    /**
     * Handling Escape
     * @param e 
     */
    onKeydown(e: KeyboardEvent) {
        //console.log(e.key);
        if (e.key == "Escape") {
            this.props.onBrowsingModeChanged(BrowsingModes.Cube);
        }
    }
}