import React, { useEffect, useState } from "react";
import { Filter } from '../Filter';
import Fetcher from '../Middle/CubeBrowser/Fetcher';
import { Tag } from './Tag';

/**
 * enum that defines range operations
 */
export enum Operation {
  GreaterThanOrEqual,
  LessThanOrEqual,
}

/**
 * functional slider component
 */
 export const Slider : any = (props:{
  tagsetName: string,
  rangeDirection: Operation,
  onFiltersChanged: (filter: Filter) => void, 
  activeFilters: Filter[],
  onFilterReplaced: (oldFilter:Filter, newFilter: Filter) => void,
  onFilterRemoved : (filterId: number) => void 
 }) => {

  const [opDirection, setOpDirection] = useState<Operation>(props.rangeDirection); //default from props
  const [displayValue, updateValue] = useState<number>(50);
  const [min, updateMin] = useState<number>(0);
  const [max, updateMax] = useState<number>(100);
  const [filterValue, updateFilterValue] = useState<number | null>(null);
  const [thisTagset, updateTagset] = useState<number>(-1);
  const [selectedFilter, updateSelection] = useState<Filter | null>(null);

  useEffect(() =>  {
    FetchTagsByTagsetName(); 
}, []);

  //when opDirection and FilterValue changes a new filter is applied
  useEffect(() => {
    console.log('op:', opDirection, 'value:',filterValue, '- Has changed')
    if(filterValue!=null){
      addFilter(filterValue)
    }
  },[opDirection, filterValue])

// initializes component
async function FetchTagsByTagsetName () {
    const response = await Fetcher.FetchTagsByTagsetName(props.tagsetName);
    //console.log(response);
    const tagsfromFetch: Tag[] = response.map((t: Tag) => {return {id: t.id, name: t.name, tagset: t.tagset}});
    //update state with tagset specific for slider instenace
    updateTagset(tagsfromFetch[0].tagset)
    //filter hierarchy structure from tags
    const tags = tagsfromFetch.filter((t:Tag) => !t.name.includes('sp_track_duration') && !t.name.includes(':'))
    //sort tags
    tags.sort((a,b) => parseInt(a.name) - parseInt(b.name));
    //set min nand max state
    updateMin(parseInt(tags[0].name))
    const lastValue = tags.length-1 //find max value
    updateMax(parseInt(tags[lastValue].name))

    //console.log(parseInt(tags[0].name))
    console.log(parseInt(tags[lastValue].name))
    console.log(tags.length)
}

/**
 * change on filterValue and opDirection triggers addFilter
 * @param value 
 */
const addFilter = (filtervalue: number) => {
  console.log('tagset',thisTagset, 'value',filtervalue)
  const filter = createRangeFilter(filtervalue)

  if (selectedFilter!=null){
    props.onFilterReplaced(selectedFilter, filter)
  } else {
    props.onFiltersChanged(filter);
  }
  updateSelection(filter)
}

/**
 * creates a range filter dependend on the direction of the operation state
 * @param value 
 * @returns range filter
 */
const createRangeFilter = (value: number) : Filter => {
  switch(opDirection as Operation){
    case Operation.GreaterThanOrEqual:
      return createFilter(thisTagset, "slider", value.toString(), max.toString());
    case Operation.LessThanOrEqual:
      return createFilter(thisTagset, "slider", min.toString(), value.toString());
    default :
      console.log("Expand operation enum!")
      return createFilter(-1, "null", "-1", "-1")
  }
}

/**
 * change on opDirection triggers addFilter()
 */
const flip = () => {
  opDirection === Operation.GreaterThanOrEqual ? setOpDirection(Operation.LessThanOrEqual) : setOpDirection(Operation.GreaterThanOrEqual)
}

/**
 * resets slider component
 */
const onClear = () => {
  if (selectedFilter !== null) {
    props.onFilterRemoved(selectedFilter.id);
    updateSelection(null);
    updateFilterValue(null)
    updateValue(50)
    setOpDirection(props.rangeDirection)
  }
}

  return (
    <div>
      {/* {props.tagsetName==="sp_track_duration" || props.tagsetName==="sp_track_popularity"? <span/>:<h5>{props.tagsetName}</h5>} */}
      <h5>{props.tagsetName}</h5>
      <input onChange={event => updateValue(event.currentTarget.valueAsNumber)}
      onMouseUp={ event => updateFilterValue(event.currentTarget.valueAsNumber)}
      type="range" min={min} max={max} value={displayValue}/>
      <p>{props.tagsetName === "sp_track_duration" ? secToMin(displayValue) : displayValue}</p>
      <br/>
      <button onClick={() => flip()}> {opDirection==Operation.GreaterThanOrEqual? ">=" : "<=" } </button>
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
    name: lower, //Filter assumes both name and max is a string
    max: upper,
  }
  return filter;
}

/**
 * formats seconds to min:sec. Used on duration tagset
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
