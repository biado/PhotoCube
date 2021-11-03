import React, { useEffect, useState } from 'react';
import { Filter } from '../../Filter';
import Fetcher from '../CubeBrowser/Fetcher';
import Tagset from '../CubeBrowser/Tagset';
import Dropdown, { Option } from 'react-dropdown';
import 'react-dropdown/style.css';
import '../../../css/BottomDock/TagsetFilter.css';

/**
 * Component for browsing and adding tagset filters. 
 */
export const TagsetDropdown = (props: {onFiltersChanged: (filter: Filter) => void, activeFilters: Filter[]}) => {

    const [options, setDropdownOptions] = useState<Option[]>([]);
    const [selectedTagset, updateSelection] = useState<Tagset | null>(null);
    const [buttonDisabled, disableButton] = useState<boolean>(true);

    useEffect(() =>  {
        fetchTagsets(); 
    }, []);

    async function fetchTagsets () {
        const response = await Fetcher.FetchTagsets();
        //console.log("from tagsetfilter", response)
        const tagsets = response.map((ts: Tagset) => {return {id: ts.id, name: ts.name }});
        setDropdownOptions(tagsets.map((ts: Tagset) => {return {value: ts.id.toString(), label: ts.name}}));
    }

    const addFilter = () => {
        const filter: Filter = createFilter(selectedTagset!.name, selectedTagset!.id, "tagset");
        if (!props.activeFilters.some(af => af.Id === filter.Id)) {
            props.onFiltersChanged(filter);
            disableButton(true);
        }
    }

    const updateDropdown = (e: Option) => {
        updateSelection({id: parseInt(e.value), name: e.label!.toString(), tags: null});
        disableButton(props.activeFilters.some(af => af.Id === parseInt(e.value)));
    }

    return (
        <div className="Filter">
            <Dropdown options={options} placeholder="Select a tagset" onChange={e => updateDropdown(e)}/>
            <button className="add button" disabled={buttonDisabled} onClick={() => addFilter()}>Add filter</button>
        </div>
    )
}

//utility function
export const createFilter = (tagName: string, id: number, type: string) => {
    const filter: Filter = {
        Id: id,
        type: type,
        name: tagName
    }
    return filter;
}