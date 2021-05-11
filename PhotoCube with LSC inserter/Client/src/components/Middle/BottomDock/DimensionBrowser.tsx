import React from 'react';
import { Filter } from '../../Filter';
import { TagsetDropdown } from './TagsetFilter';

export const DimensionBrowser = (props: {onFiltersChanged: (filter: Filter) => void, activeFilters: Filter[] }) => {
    return (
        <div id="dimension-browser">
            <TagsetDropdown activeFilters={props.activeFilters.filter(af => af.type === "tagset")} onFiltersChanged={props.onFiltersChanged}/>
        </div>
    )
}
