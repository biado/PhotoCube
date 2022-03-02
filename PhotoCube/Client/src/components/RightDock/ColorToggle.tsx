import React, { useEffect, useState } from 'react';

/**
 * functional slider component
 */
 export const ColorToggle : any = (props:{
    onColorChange: (color:boolean) => void,
   }) => {

    const [color, updateColor] = useState<boolean>(false);
  
    useEffect(() =>  {
       //this.handleClick = this.handleClick.bind(this);  
}, []);
  
    //when opDirection and FilterValue changes a new filter is applied
    useEffect(() => {
      console.log(color, '- Has changed')
      props.onColorChange(color)
    },[color])
  
  // initializes component
  async function FetchTagsByTagsetName () {

  }
  const handleClick = () =>{
    color ? updateColor(false) : updateColor(true) 
  }
      return (
        <div>
            <p> color </p>
            <button onClick={handleClick}>        
                {color ? 'ON' : 'OFF'}
            </button>
        </div>
      );
     
  } 
