import React, { useEffect, useState } from 'react';
import { Filter } from '../Filter';
import { createFilter } from '../Middle/BottomDock/TagsetFilter';
import Fetcher from '../Middle/CubeBrowser/Fetcher';
import { Tag } from './Tag';
import Dropdown, { Option } from 'react-dropdown';
import '../../css/LeftDock/TagFilter.css';

/**
 * Component for browsing and adding date filters.
 */
 export const TagDropdown = (props: {
     tagsetName: string, onFiltersChanged: (filter: Filter) => void, activeFilters: Filter[]}) => {

    const [options, setDropdownOptions] = useState<Option[]>([]);
    const [selectedTag, updateSelection] = useState<Tag | null>(null);
    const [buttonDisabled, disableButton] = useState<boolean>(true);

    useEffect(() =>  {
        FetchTagsByTagsetName(); 
    }, []);

    async function FetchTagsByTagsetName () {
        const response = await Fetcher.FetchTagsByTagsetName(props.tagsetName);
        const tags = response.map((t: Tag) => {return {Id: t.Id, Name: t.Name }});
        setDropdownOptions(tags.map((t: Tag) => {return {value: t.Id.toString(), label: t.Name}}));
    }

    const addFilter = () => {
        const filter: Filter = createFilter(selectedTag!.Name, selectedTag!.Id, "tag", "", "");
        if (!props.activeFilters.some(af => af.Id === filter.Id)) {
            props.onFiltersChanged(filter);
            disableButton(true);
        }
    }

    const updateDropdown = (e: Option) => {
        updateSelection({Id: parseInt(e.value), Name: e.label!.toString()});
        disableButton(props.activeFilters.some(af => af.Id === parseInt(e.value)));
    }

    return (
        <div className="Filter">
            <Dropdown options={options} placeholder={"Select "+ props.tagsetName} onChange={e => updateDropdown(e)}/>
            <button className="add button" disabled={buttonDisabled} onClick={() => addFilter()}>Add filter</button>
        </div>
    )
}