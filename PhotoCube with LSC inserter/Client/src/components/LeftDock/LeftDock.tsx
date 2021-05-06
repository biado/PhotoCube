import React, { Component } from 'react';
import '../../css/LeftDock.css';
import { Filter } from '../Filter';

/**
 * LeftDock is the left portion of the interface.
 * PhotoCubeClient.tsx contains: LeftDock, Middle and RightDock.
 */
export default class LeftDock extends Component<{
        hideControls: boolean,
        onFiltersChanged : (filters: Filter[]) => void
    }>{
    render() {
        return (
            <div id="LeftDock">
	  		</div>
        );
        //Not in use: <BrowsingStateLoader className={classNames}/>
    }
}