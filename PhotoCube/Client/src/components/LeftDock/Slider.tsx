import React, { useEffect, useState } from "react";
import { Filter } from '../Filter';
import Fetcher from '../Middle/CubeBrowser/Fetcher';
import { Tag } from './Tag';
import { Operation } from "./Operation";
//import { createFilter } from '../Middle/BottomDock/TagsetFilter';
import '../../css/LeftDock/DayOfWeekFilter.css';
import { AiFillSliders } from "react-icons/ai";
import { EnumType, idText } from "typescript";

/**
 * 
 */
 export const Slider : any = (props:{
  tagsetName: string,
  rangeDirection: Operation,
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
    //   addFilter(tags[currValue]) //*use default value as filter?
    // }

    //console.log(parseInt(tags[0].name))
    //console.log(parseInt(tags[lastValue].name))
    //console.log(tags)
}

const addFilter = (option: Tag) => {
  console.log(option)
  var filter : Filter = createFilter(-1, "null", "-1", "-1");

  switch(props.rangeDirection as Operation){
    case Operation.GreaterThanOrEqual:
      filter = createFilter(option.tagset, "slider", option.name, max.toString());
      break;
    case Operation.LessThanOrEqual:
      filter = createFilter(option.tagset, "slider", min.toString(), option.name);
      break;
    default : 
      console.log("Update operation enum")
      break; 
  }

  console.log("filter",filter)
  console.log("af", props.activeFilters)

  if (selectedFilter!=null){
    props.onFilterReplaced(selectedFilter, filter)
  } else {
    props.onFiltersChanged(filter);
  }
  updateSelection(filter)
}

const applyFilter = (value: number) => {
  console.log(value)
  for (let index = 0; index < allTags.length; index++) {
    const element = allTags[index];
    if (parseInt(element.name) == value){
      addFilter(element);
      break
    }
    if (parseInt(element.name) > value){ // no exacct match, eg track_duration
      const t : Tag= {'id':element.tagset, 'name':value.toString(), 'tagset':element.tagset}
      addFilter(t); // Tag t doesnt exist in db
      break
    }
  }
}

const onClear = () => {
  if (selectedFilter !== null) {
      props.onFilterRemoved(selectedFilter.id);
      updateSelection(null);
      updateValue(50)
  }
}

  return (
      <div>
        {props.tagsetName==="sp_track_duration" ? <span/>:<h5>{props.tagsetName}</h5>}
        <input onChange={event => updateValue(event.currentTarget.valueAsNumber)} 
        onMouseUp={ event => applyFilter(event.currentTarget.valueAsNumber)}  
        type="range" min={min} max={max} value={currValue}/>
        <p>{props.tagsetName==="sp_track_duration" ? secToMin(currValue) : currValue}</p>
        <button onClick={() => onClear()}> Clear </button>
      </div>
  );
} 

/**
 * Slider specific filter creation with lower and upper bound
 * @param tagSet 
 * @param type 
 * @param lower 
 * @param upper 
 * @returns returns new created filter
 */
const createFilter = (tagSet: number, type: string, lower: string, upper: string ) : Filter => {
  const filter: Filter = {
      id: tagSet,
      type: type,
      name: lower, //Filter assumes name is a string
      max: upper,
  }
  return filter;
}

/**
 * formats seconds to min:sec
 * @param sec 
 * @returns 
 */
const secToMin = (sec: number) : string => {
  const secRemainder : number = sec % 60;
  const oneDigit : boolean = secRemainder < 10
  const min : number = Math.floor(sec / 60)
  var minAndSec: string = '';
  oneDigit? minAndSec=`${min}:0${secRemainder}` : minAndSec=`${min}:${secRemainder}`
  return minAndSec
}
