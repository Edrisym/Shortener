import { blue } from '@mui/material/colors';
import { createTheme } from '@mui/material/styles';


export default createTheme({
    palette:{
        primary:{
            main: "#FF0"
        },
        background:{
            default:"#123727"
        },
    },
    components: {
      // Name of the component
      MuiButton: {
        styleOverrides: {
          // Name of the slot
          root: {
            // Some CSS
            // fontSize: '1rem',
          },
        },
      },
    },
  });  