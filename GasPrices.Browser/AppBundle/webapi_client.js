export async function getAsync(url) {
    try {
        const response = await fetch(url); // Use the provided URL parameter
        const test = JSON.stringify(await response.json());
        return test;
    } catch (error) {
        console.error(error);
        return null; // Return a default value or handle the error appropriately
    }
}