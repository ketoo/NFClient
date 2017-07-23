




    var teclado: AnimationClip;
    
    function Update () {
    
    if (Input.GetKey("1"))
    		{
    		GetComponent.<Animation>().Play ("run");}
    		
    else if(Input.GetKey("2"))
    		{
  			  GetComponent.<Animation>().Play ("attack");}
  			  
  	else if(Input.GetKey("3"))
    		{
  			  GetComponent.<Animation>().Play ("walk");}		  

    else if(Input.GetKey("4"))
    		{
  			  GetComponent.<Animation>().Play ("jump");}
  			  
  	else 
   			 {
    		GetComponent.<Animation>().Play ("idle");}

   	 }
    



    
    
    
    
    
