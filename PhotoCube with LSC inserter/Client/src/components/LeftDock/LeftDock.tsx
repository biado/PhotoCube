import React, { Component } from 'react';
import '../../css/LeftDock/LeftDock.css';
import { Filter } from '../Filter';
import { TagSearcher } from './TagFilter';

/**
 * LeftDock is the left portion of the interface.
 * PhotoCubeClient.tsx contains: LeftDock, Middle and RightDock.
 */
export default class LeftDock extends Component<{
        hideControls: boolean,
        onFiltersChanged : (filters: Filter) => void,
        activeFilters: Filter[]
    }>{
    render() {
        let visibility: string = this.props.hideControls ? "hide" : "";
        return (
            <div id="LeftDock">
                <div className="tag dropdown">
                    <h4 className="Header">Tag filter:</h4>
                    <TagSearcher className={visibility} onFiltersChanged={this.props.onFiltersChanged} activeFilters={this.props.activeFilters}></TagSearcher>
                </div>
	  		</div>
        );
        //Not in use: <BrowsingStateLoader className={classNames}/>
    }
}