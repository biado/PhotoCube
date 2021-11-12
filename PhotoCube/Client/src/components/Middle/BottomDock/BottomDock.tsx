import React, { useState } from 'react';
import '../../../css/BottomDock/BottomDock.css';
import { MdExpandLess, MdExpandMore } from 'react-icons/md';
import { VscBrowser } from 'react-icons/vsc';
import { DimensionBrowser } from './DimensionBrowser';
import { Filter } from '../../Filter';

/**
 * Component representing the bottom dock. 
 * Can be expanded and collapsed, hiding the CubeBrowser.
 */
export const BottomDock = 
    (props: {onFiltersChanged: (filter: Filter) => void,
        activeFilters: Filter[], hideControls: boolean}) => {
    const [isExpanded, expand] = useState(false);

    let visibility: string = props.hideControls ? "hide" : "";

    return(
        <div className={isExpanded ? "bottom dock expanded" : "bottom dock " + visibility}>
            <div className="dimensionbrowser header" onClick={e => expand(!isExpanded)}>
                <div className="dock name">
                    <VscBrowser id="browser-icon" />
                    <h5>Dimension Browser</h5>
                </div>
                {isExpanded ? <MdExpandMore className="expand"/> : <MdExpandLess className="expand"/>}
            </div>
            {isExpanded ? <DimensionBrowser activeFilters={props.activeFilters} onFiltersChanged={props.onFiltersChanged}/> : null}
        </div>
    )
};