export async function getAsync(url) {
    try {
        const response = await fetch(url);
        return JSON.stringify(await response.json());
    } catch (error) {
        console.error(error);
        return null;
    }
}