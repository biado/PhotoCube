import React, { useEffect, useState } from "react";
import { Filter } from '../Filter';
import Fetcher from '../Middle/CubeBrowser/Fetcher';
import { Tag } from './Tag';
import { createFilter } from '../Middle/BottomDock/TagsetFilter';
import '../../css/LeftDock/DayOfWeekFilter.css';
import { AiFillSliders } from "react-icons/ai";
import { idText } from "typescript";

/**
 * 
 */
 export const Slider : any = (props:{
  tagsetName: string
  onFiltersChanged: (filter: Filter) => void, 
  activeFilters: Filter[],
  onFilterReplaced: (oldFilter:Filter, newFilter: Filter) => void,
  onFilterRemoved : (filterId: number) => void 
 }) => {

  const [currValue, updateValue] = useState<number>(50);
  const [min, updateMin] = useState<number>(0);
  const [max, updateMax] = useState<number>(100);
  const [allTags, updateTags] = useState<Tag[]>([]);
  //const [previousFilter, updatePrevious] = useState<Filter | null>(null);
  const [selectedFilter, updateSelection] = useState<Filter | null>(null);

  useEffect(() =>  {
    FetchTagsByTagsetName(); 
}, []);

async function FetchTagsByTagsetName () {
    const response = await Fetcher.FetchTagsByTagsetName(props.tagsetName);
    //console.log(response);
    const tags: Tag[] = response.map((t: Tag) => {return {id: t.id, name: t.name, tagset: t.tagset}});
    //sort tags
    tags.sort((a,b) => parseInt(a.name) - parseInt(b.name));
    updateTags(tags) //set state
    updateMin(parseInt(tags[0].name))
    const lastValue = tags.length-1 //find max value
    updateMax(parseInt(tags[lastValue].name))

    // if(selectedFilter==null){
    //   addFilter(tags[currValue]) //use default value as filter
    // }

    //console.log(parseInt(tags[0].name))
    //console.log(parseInt(tags[lastValue].name))
    //console.log(tags)
}

const addFilter = (option: Tag) => {
  console.log(option)
  //const filter: Filter = createFilter(option.name, option.id, "slider");
  const filter: Filter = createFilter(option.name, option.tagset, "slider");
  console.log("filter",filter)
  console.log("af", props.activeFilters)
  //console.log(!props.activeFilters.some(af => af.id === filter.id && af.name === filter.name))
    if (!props.activeFilters.some(af => af.id === filter.id && af.name === filter.name)) { //check on both name and id - becase tagset_id is stored as id
      //remove past filter
      //props.activeFilters.
      if (selectedFilter!=null){
        props.onFilterReplaced(selectedFilter, filter)
      } else {
        props.onFiltersChanged(filter);
      }
      updateSelection(filter)
      // less than or equal
  }
}

const changeValue = (value: number) => {
  updateValue(value)
  for (let index = 0; index < allTags.length; index++) {
    const element = allTags[index];
    if (parseInt(element.name) == value){
      addFilter(element)
    }
  }
}

  return (
      <div>
        <h5>{props.tagsetName}</h5>
        {/*<input onMouseUp={ event => changeValue(event.currentTarget.valueAsNumber)} type="range" min={min} max={max} value={currValue}/>*/}
        <input onChange={event => changeValue(event.target.valueAsNumber)} type="range" min={min} max={max} value={currValue}/>
        <p>{currValue}</p>
      </div>
  );
} 
