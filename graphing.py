import pandas as pd
import matplotlib.pyplot as plt

def plot_csv_data(file_path):
   # Read the CSV file
    df = pd.read_csv(file_path, header=None, names=['x', 'y', 'z'])
    
    # Create the plot with two y-axes
    fig, ax1 = plt.subplots(figsize=(12, 6))
    ax2 = ax1.twinx()  # Create a twin Axes sharing the x-axis
    
    # Set the background color to light grey
    fig.patch.set_facecolor('#f0f0f0')  # Light grey color
    ax1.set_facecolor('#f0f0f0')  # Light grey color

    # Plot x and y on the left y-axis
    ax1.plot(df['x'], label='White %', color='green')
    ax1.plot(df['y'], label='Black %', color='black')
    
    # Plot z on the right y-axis
    ax2.plot(df['z'], label='World Temp (°C)', color='red')
    
    # Customize the plot
    ax1.set_xlabel('Ticks')
    ax1.set_ylabel('Daisy percentages')
    ax2.set_ylabel('Temperature (°C)')
    
    # Set the range for the right y-axis (z values)
    ax2.set_ylim(0, 50)
    
    # Add legends
    ax1.legend(loc='upper left')
    ax2.legend(loc='upper right')
    
    plt.title('Daisy World Simulation')
    
    # Adjust layout to prevent clipping of labels
    plt.tight_layout()
    
    plt.savefig("worldstats.png", dpi=300, bbox_inches='tight')
    # Display the plot
    plt.show()

plot_csv_data("world stats.csv")