import React from 'react';
import { Filter } from '../../Filter';
import { HierarchyExplorer } from './HierarchyFilter';
import { TagsetDropdown } from './TagsetFilter';
import '../../../css/DimensionBrowser.css'

export const DimensionBrowser = (props: {onFiltersChanged: (filter: Filter) => void, activeFilters: Filter[] }) => {
    return (
        <div id="dimension-browser">
            <div className="tagset dropdown">
                <h4 className="Header">Tagset filter:</h4>
                <TagsetDropdown activeFilters={props.activeFilters.filter(af => af.type === "tagset")} onFiltersChanged={props.onFiltersChanged}/>
            </div>
            <div className="hierarchy explorer">
                <h4 className="Header">Hierarchy filter:</h4>
                <HierarchyExplorer/>
            </div>
        </div>
    )
}
